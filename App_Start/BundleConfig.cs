using System.Web.Optimization;

namespace Registrar
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // JS
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.js"));

            // ✅ Ajoute le JS des sélections
            bundles.Add(new ScriptBundle("~/bundles/selections").Include(
                "~/Scripts/Selections.js"));

            // CSS
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.min.css",
                "~/Content/site.css",
                "~/Content/Registrar.css",
                "~/Content/Selections.css"));
        }
    }
}