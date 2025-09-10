using System;
using System.Collections.Generic;

namespace Goober.Base.Comparators
{
    /// <summary>
    /// Представляет операцию естественного сравнения строк 
    /// </summary>
    public class StringNaturalComparer : IComparer<string>
    {
        /// <summary>
        /// Сравнивает два указанных объекта string и возвращает целое число, указывающее их относительную позицию в порядке сортировки.
        /// </summary>
        /// <param name="strX">Первая строка, используемая в сравнении</param>
        /// <param name="strY">Вторая строка, используемая в сравнении</param>
        /// <returns>
        /// Целое число, указывающее лексическую связь между двумя подстроками. <br/> <br/>
        /// Меньше ноля - подстрока в <paramref name="strX"/> предшествует подстроке в <paramref name="strY"/> в порядке сортировки. <br/>
        /// Ноль - подстроки выполняются в той же позиции или порядке сортировки или length равно нолю. <br/>
        /// Больше ноля - подстрока в  <paramref name="strX"/> следует подстроке в <paramref name="strY"/> в порядке сортировки. <br/>
        /// </returns>
        /// <remarks>
        /// Строки, содержащие числа, сравниваются с учетом значения числа, а не посимвольно.<br/>
        /// Например: A1B2 &lt; A1B11 <br/><br/>
        /// Строки, содержащие числа с незначащими ведущими нолями, сравниваются без учета нолей.<br/>
        /// Например: A001B2 = A1B002  <br/><br/>
        /// Строки, состоящие из одинаковых символов и чисел, но с разным количеством незначащих ведущих нолей, сравниваются по длине.<br/>
        /// Например: A01B2 &lt; A001B2  <br/><br/>
        /// </remarks>
        public int Compare(string strX, string strY)
        {
            if (strX is null && strY is null)
                return 0;

            if (strX is null)
                return 1;

            if (strY is null)
                return -1;

            var lenStrX = strX.Length;
            var lenStrY = strY.Length;

            for (int iX = 0, iY = 0; iX < lenStrX && iY < lenStrY; iX++, iY++)
            {
                if (char.IsDigit(strX[iX]) && char.IsDigit(strY[iY]))
                {
                    int indexStartSubX = -1; //индекс начала подстроки X состоящей из цифр
                    int indexStartSubY = -1; //индекс начала подстроки Y состоящей из цифр

                    for (; iX < lenStrX && char.IsDigit(strX[iX]); iX++)
                    {
                        if (indexStartSubX == -1 && strX[iX] != '0') //не учитываем незначащие нули
                            indexStartSubX = iX;
                    }

                    for (; iY < lenStrY && char.IsDigit(strY[iY]); iY++)
                    {
                        if (indexStartSubY == -1 && strY[iY] != '0') //не учитываем незначащие нули
                            indexStartSubY = iY;
                    }

                    var isZerosStrX = indexStartSubX == -1; //подстрока состоит из всех нолей
                    var isZerosStrY = indexStartSubY == -1; //подстрока состоит из всех нолей

                    if (isZerosStrX && isZerosStrY)
                        continue;

                    if (isZerosStrX && isZerosStrY == false)
                        return -1;

                    if (isZerosStrY && isZerosStrX == false)
                        return 1;

                    var lenSubX = iX - indexStartSubX; //разряд числа в подстроке X
                    var lenSubY = iY - indexStartSubY; //разряд числа в подстроке Y 

                    if (lenSubX > lenSubY) //сравнение по разрядам числа
                        return 1;

                    if (lenSubX < lenSubY) //сравнение по разрядам числа
                        return -1;

                    var subX = strX.AsSpan(indexStartSubX, iX - indexStartSubX);
                    var subY = strY.AsSpan(indexStartSubY, iY - indexStartSubY);

                    for (int i = 0; i < subX.Length; i++)
                    {
                        if (subX[i] == subY[i])
                            continue;

                        return subX[i] - subY[i];
                    }
                }

                if (iX < lenStrX && iY < lenStrY && strX[iX] != strY[iY])
                    return strX[iX] > strY[iY] ? 1 : -1;
            }

            return lenStrX - lenStrY;
        }
    }
}
