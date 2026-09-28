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
            Higher,
            SecondaryVocational,
            SecondaryGeneral,
            Other
        }
    }
}
