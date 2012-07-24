using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTC.SharePoint.Evolution.Core
{
    public struct SPEvolutionInfo
        {
            /// <summary>
            /// Инициализация
            /// </summary>
            /// <param name="type">Тип, из которого извлекается информация о миграции</param>
            public SPEvolutionInfo(Type type)
            {                
                if (type == null)
                    throw new ArgumentNullException("Evolution Instance Type is null");
                if (!typeof(IEvolution).IsAssignableFrom(type))
                    throw new ArgumentNullException("SPEvolution class must implement IEvolution");

                SPEvolutionAttribute attribute = AttributesHelper.GetEvolutionAttribute(type);
               
                if (attribute == null)
                    throw new ArgumentNullException("Attribute of Evolution Not Found");

                Type = type;
                Version = attribute.Version;
                Ignore = attribute.Ignore;
            }

            /// <summary>
            /// Тип миграции
            /// </summary>
            public readonly Type Type;

            /// <summary>
            /// Версия
            /// </summary>
            public readonly long Version;

            /// <summary>
            /// Признак: пропустить миграцию при выполнении
            /// </summary>
            public readonly bool Ignore;
        }
    public class SPEvolutionInfoComparer : IComparer<SPEvolutionInfo>
    {
        /// <summary>
        /// Признак, что проводится сортировка по возрастанию
        /// </summary>
        private readonly bool ascending;

        /// <summary>
        /// Инициализация
        /// </summary>
        /// <param name="ascending">Порядок сортировки (true = по возрастанию, false = по убыванию)</param>
        public SPEvolutionInfoComparer(bool ascending)
        {
            this.ascending = ascending;
        }

        /// <summary>
        /// Сравнение двух миграций
        /// </summary>
        public int Compare(SPEvolutionInfo x, SPEvolutionInfo y)
        {
            return ascending
                ? x.Version.CompareTo(y.Version)
                : y.Version.CompareTo(x.Version);
        }
    }

   
}
