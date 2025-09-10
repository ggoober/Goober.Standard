using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Goober.Base.Enums;
using Goober.Base.Models;

namespace Goober.Base.Extensions;

public static class QueryableExtensions
{
    /// <summary>
    /// Добавляет к запросу сортировку по указанному полю
    /// </summary>
    /// <param name="query">Исходный запрос</param>
    /// <param name="field">Строковое имя поля, принадлежащего модели <typeparamref name="TModel"/></param>
    /// <param name="sortOrder">Направление сортировки: по возрастанию или убыванию</param>
    /// <typeparam name="TModel">Тип элемента, возвращаемого запросом</typeparam>
    /// <returns>Отсортированный запрос</returns>
    public static IQueryable<TModel> OrderBy<TModel>(
        this IQueryable<TModel> query,
        string field,
        SortOrder sortOrder = SortOrder.ASC)
    {
        var expression = query.Expression;
        var parameter = Expression.Parameter(typeof(TModel), "x");
        var selector = Expression.PropertyOrField(parameter, field);
        var method = sortOrder == SortOrder.DESC ? "OrderByDescending" : "OrderBy";
        expression = Expression.Call(typeof(Queryable), method,
            new [] { query.ElementType, selector.Type },
            expression, Expression.Quote(Expression.Lambda(selector, parameter)));

        return query.Provider.CreateQuery<TModel>(expression);
    }

    /// <summary>
    /// Добавляет к запросу сортировку по нескольким указанным полям
    /// </summary>
    /// <param name="query">Исходный запрос</param>
    /// <param name="sortedColumns">Список параметров сортировки (имя поля и направление)</param>
    /// <param name="throwException">
    /// Требуется ли генерировать исключение.
    /// При значении <see langword="false"/> некорректные параметры сортировки будут пропущены.
    /// По умолчанию <see langword="true"/>
    /// </param>
    /// <typeparam name="TModel">Тип элемента, возвращаемого запросом</typeparam>
    /// <returns>Отсортированный запрос</returns>
    /// <exception cref="InvalidOperationException">Одно из указанных для сортировки полей отсутствует у типа <typeparamref name="TModel"/></exception>
    public static IQueryable<TModel> OrderByMultiple<TModel>(
        this IQueryable<TModel> query,
        IEnumerable<ISortModel> sortedColumns = null,
        bool throwException = true)
    {
        sortedColumns = sortedColumns?.ToList();

        if (sortedColumns == null || !sortedColumns.Any())
            return query;

        var firstTime = true;

        var type = typeof(TModel);
        var parameter = Expression.Parameter(type, "x");

        foreach (var sortedColumn in sortedColumns)
        {
            var property = type.GetProperty(sortedColumn.FieldName);
            if (property == null)
            {
                if (throwException)
                    throw new InvalidOperationException($"Поле '{sortedColumn.FieldName}' отсутствует у типа '{type.FullName}'.");
                continue;
            }

            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var expression = Expression.Lambda(propertyAccess, parameter);

            string methodName;
            if (firstTime)
            {
                methodName = sortedColumn.SortOrder == SortOrder.ASC
                    ? nameof(Queryable.OrderBy)
                    : nameof(Queryable.OrderByDescending);
                firstTime = false;
            }
            else
            {
                methodName = sortedColumn.SortOrder == SortOrder.ASC
                    ? nameof(Queryable.ThenBy)
                    : nameof(Queryable.ThenByDescending);
            }

            var callExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new [] { type, expression.Body.Type },
                query.Expression,
                expression
            );

            query = query.Provider.CreateQuery<TModel>(callExpression);
        }

        return query;
    }

    /// <summary>
    /// Добавляет пагинацию к запросу
    /// </summary>
    /// <param name="query">Исходный запрос</param>
    /// <param name="page">Параметры страницы: номер и размер. При отсутствии (значение параметра <see langword="null"/>) возвращается исходный запрос</param>
    /// <param name="throwException">
    /// Требуется ли генерировать исключение.
    /// При значении <see langword="false"/> некорректные параметры страницы не будут применены.
    /// По умолчанию <see langword="true"/>.
    /// </param>
    /// <typeparam name="TModel">Тип элемента, возвращаемого запросом</typeparam>
    /// <returns>Модифицированный запрос, выгружающий указанную страницу элементов</returns>
    /// <exception cref="ArgumentException">Номер либо размер страницы меньше, чем 1</exception>
    public static IQueryable<TModel> TakePage<TModel>(
        this IQueryable<TModel> query,
        IPageModel page,
        bool throwException = true)
    {
        if (page == null)
        {
            return query;
        }
        if (page.PageNumber < 1 || page.PageSize < 1)
        {
            if (throwException)
                throw new ArgumentException($"Некорректные параметры страницы: номер ({page.PageNumber}) и размер ({page.PageSize}) страницы не могут быть меньше 1.", nameof(page));
            return query;
        }

        var offset = (page.PageNumber - 1) * page.PageSize;

        var pagingQuery = query
            .Skip(offset)
            .Take(page.PageSize);

        return pagingQuery;
    }
}
