using System;
using Microsoft.Extensions.Configuration;

namespace Goober.Base.Extensions
{
    /// <summary>
    /// Вспомогательный класс для чтения файла конфигурации приложения.
    /// </summary>
    public static class ConfigSettingsExtension
    {
        /// <summary>
        /// Возвращает значение (объект типа <typeparamref name="T"/>) из файла конфигурации по ключу <paramref name="keyPath"/>.
        /// В случае, если во время извлечения значения возникают проблемы (значение отсутствует,
        /// или не может быть преобразовано в запрошенный тип), возвращает значение, переданное
        /// в качестве значения по умолчанию.
        /// </summary>
        /// <typeparam name="T">Тип предполагаемого значения внутри файла конфигурации</typeparam>
        /// <param name="configuration">Конфигурация приложения</param>
        /// <param name="keyPath">Ключ-путь до искомого значения (объекта) или секции файла конфигурации</param>
        /// <param name="defaultValue">Значение по умолчанию, которое будет возвращено в качестве результата работы метода,
        /// если при извлечении из файла возникнут проблемы</param>
        /// <returns>Искомое значение конфигурационного свойства внутри файла конфигурации приложения</returns>
        public static T GetSettingValueByKeyOrDefault<T>(this IConfiguration configuration, string keyPath, T defaultValue)
        {
            var sectionData = configuration.GetSection(keyPath);
            return sectionData?.Exists() == true
                ? sectionData.Get<T>()
                : defaultValue;
        }

        /// <summary>
        /// Возвращает значение (объект типа <typeparamref name="T"/>) из файла конфигурации по ключу <paramref name="keyPath"/>.
        /// или выбрасывает исключение в случае, если нет возможности считать значение корректно.
        /// </summary>
        /// <typeparam name="T">Тип предполагаемого значения внутри файла конфигурации</typeparam>
        /// <param name="configuration">Конфигурация приложения</param>
        /// <param name="keyPath">Ключ-путь до искомого значения (объекта) или секции файла конфигурации</param>
        /// <returns>Искомое значение конфигурационного свойства внутри файла конфигурации приложения, в случае успешного считывания</returns>
        public static T GetSettingValueByKey<T>(this IConfiguration configuration, string keyPath)
        {
            if(string.IsNullOrEmpty(keyPath))
                throw new ArgumentException(nameof(keyPath));

            var sectionData = configuration.GetSection(keyPath);
            return sectionData?.Exists() == true
                ? sectionData.Get<T>()
                : throw new InvalidOperationException($"Can not find configuration section by {keyPath}");
        }
    }
}