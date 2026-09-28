using System.ComponentModel;

namespace StudentAccounting.Domain
{
    /// <summary>
    /// Перечисления для доменных моделей системы учета обучающихся
    /// </summary>
    public class Enums
    {
        /// <summary>
        /// Статус сотрудника (активен или уволен)
        /// </summary>
        public enum EmployeeStatus
        {
            [Description("Активен")]
            Active,

            [Description("Уволен")]
            Dismissed
        }

        /// <summary>
        /// Уровень образования (высшее, средне-профессиональное, среднее общее, иное)
        /// </summary>
        public enum EducationLevel
        {
            [Description("Высшее")]
            Higher,

            [Description("Средне-профессиональное")]
            SecondaryVocational,

            [Description("Среднее общее")]
            SecondaryGeneral,

            [Description("Иное")]
            Other
        }

        /// <summary>
        /// Требования к образованию
        /// </summary>
        public enum RequirementsEducation
        {
            [Description("Не требуется")]
            NotRequired,

            [Description("Среднее общее")]
            SecondaryGeneral,

            [Description("Средне-профессиональное")]
            SecondaryVocational,

            [Description("Высшее")]
            Higher,

            [Description("Иное")]
            Other
        }

        /// <summary>
        /// Переодичность повторного обучения
        /// </summary>
        public enum RetrainingPeriodicity
        {
            [Description("Не требуется")]
            NotRequired,

            [Description("1 год")]
            OneYear,

            [Description("3 года")]
            ThreeYears,

            [Description("5 лет")]
            FiveYears,

            [Description("Произвольный срок (в месяцах)")]
            CustomInMonths
        }

        /// <summary>
        /// Единица измерения срока обучения
        /// </summary>
        public enum DurationUnit
        {
            [Description("Часов")]
            Hours,

            [Description("Дней")]
            Days
        }

        /// <summary>
        /// Статус программы обучения.
        /// </summary>
        public enum ProgramStatus
        {
            [Description("Активна")]
            Active,

            [Description("Архив")]
            Archive
        }

    }
}
