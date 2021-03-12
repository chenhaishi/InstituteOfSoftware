using System.Web.Mvc;

namespace SiliconValley.InformationSystem.Web.Areas.InternalTraining
{
    public class InternalTrainingAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "InternalTraining";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "InternalTraining_default",
                "InternalTraining/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}