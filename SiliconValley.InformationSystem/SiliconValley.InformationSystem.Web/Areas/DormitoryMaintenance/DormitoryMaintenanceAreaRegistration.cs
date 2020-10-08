using System.Web.Mvc;

namespace SiliconValley.InformationSystem.Web.Areas.DormitoryMaintenance
{
    public class DormitoryMaintenanceAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "DormitoryMaintenance";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "DormitoryMaintenance_default",
                "DormitoryMaintenance/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}