using ProfileSample.DAL;
using ProfileSample.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ProfileSample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (var context = new ProfileSampleEntities())
            {
                var model = context.ImgSources
                    .Take(20)
                    .Select(item => new ImageModel()
                    {
                        Data = item.Data,
                        Name = item.Name
                    })
                    .ToList();

                return View(model);
            }
        }

        public async Task<ActionResult> Convert()
        {
            var files = Directory.GetFiles(Server.MapPath("~/Content/Img"), "*.jpg");

            using (var context = new ProfileSampleEntities())
            {
                foreach (var file in files)
                {
                    using (var stream = new FileStream(file, FileMode.Open))
                    {
                        byte[] buff = new byte[stream.Length];

                        await stream.ReadAsync(buff, 0, (int)stream.Length);

                        var entity = new ImgSource()
                        {
                            Name = Path.GetFileName(file),
                            Data = buff,
                        };

                        context.ImgSources.Add(entity);
                        await context.SaveChangesAsync();
                    }
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}