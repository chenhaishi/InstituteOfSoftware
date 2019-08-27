//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace SiliconValley.InformationSystem.Entity.MyEntity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table(name: "SchoolYearPlan")]
    public partial class SchoolYearPlan
    {
        [Key]
        public int ID { get; set; }
        /// <summary>
        /// 渠道预计人数
        /// </summary>
        public Nullable<int> MarketForecast { get; set; }
        /// <summary>
        /// 口碑预计人数
        /// </summary>
        public Nullable<int> Estimatednumberofwordofmouth { get; set; }
        /// <summary>
        /// 代理预计人数
        /// </summary>
        public Nullable<int> Estimatednumberofagents { get; set; }
        /// <summary>
        /// 网络预计人数
        /// </summary>
        public Nullable<int> PredictedNetworkNumber { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remak { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 计划定制时间
        /// </summary>
        public System.DateTime PlanDate { get; set; }

        public Nullable<bool> IsDel { get; set; }
    
        
    }
}
