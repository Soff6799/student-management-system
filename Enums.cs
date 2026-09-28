using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;


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

        public enum EducationLevel
        {
            Higher,
            SecondaryVocational,
            SecondaryGeneral,
            Other
        }
    }
}
