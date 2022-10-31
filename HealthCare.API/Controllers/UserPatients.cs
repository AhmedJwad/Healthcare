using dotnetCHARTING;
using HealthCare.API.Data;
using HealthCare.API.Data.Entities;
using HealthCare.API.Helpers;
using HealthCare.API.Models;
using HealthCare.Common.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisioForge.MediaFramework.FFMPEGCore.Arguments;

namespace HealthCare.API.Controllers
{
    public class UserPatients : Controller
    {
        private readonly DataContext _context;
        private readonly ICombosHelper _combosHelper;
        private readonly IconverterHelper _converterhleper;
        private readonly IBlobHelper _blobHelper;
        private readonly IuserHelper _userhelper;


        public UserPatients(DataContext context, IuserHelper userhelper, ICombosHelper combosHelper,
            IconverterHelper converterhleper, IBlobHelper blobHelper)
        {
           _context = context;
           _combosHelper = combosHelper;
          _converterhleper = converterhleper;
           _blobHelper = blobHelper;
            _userhelper = userhelper;
        }

        public IuserHelper Userhelper { get; }

        public async Task<IActionResult> Index()
        {

            return View(await _context.Users.Include(x=>x.UserPatients).ThenInclude(x => x.Patients)
                .Where(x => x.userType == UserType.Patient).ToListAsync());

        }

        public IActionResult Create()
        {            
            UserViewModel model = new()
            {
                Id = Guid.NewGuid().ToString(),
                
            };

            return View(model);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel model)
        {

            if (ModelState.IsValid)
            {
                Guid imageId = Guid.Empty;
                if (model.ImageFile != null)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");

                }
                User user = await _converterhleper.ToUserAsync(model, imageId, true);
                user.userType = UserType.Patient;
                await _userhelper.AddUserAsync(user, "123456");
                await _userhelper.AddUsertoRoleAsync(user, UserType.Patient.ToString());
                var userpatient = new UserPatient
                {                  
                    User = user,
                    Patients=new List<Patient>(),
                };
                _context.UserPatients.Add(userpatient);
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        public async Task<IActionResult> Edit(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }
            User user = await _userhelper.GetUserAsync(Guid.Parse(Id));
            if (user == null)
            {
                return NotFound();
            }

            UserViewModel model = _converterhleper.ToUserViewModel(user);


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserViewModel model)
        {

            if (ModelState.IsValid)
            {
                Guid imageId = model.ImageId;
                if (model.ImageFile != null)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                }


                User user = await _converterhleper.ToUserAsync(model, imageId, false);
                await _userhelper.UpdateUserAsync(user);
                return RedirectToAction(nameof(Index));
            }


            return View(model);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            User user = await _userhelper.GetUserAsync(Guid.Parse(id));
            if (user == null)
            {
                return NotFound();
            }

            if (user.ImageId != Guid.Empty)
            {
                await _blobHelper.DeleteBlobAsync(user.ImageId, "users");
            }
            UserPatient userpatient = await _context.UserPatients.Include(x => x.User)
                .FirstOrDefaultAsync(x => x.User.Id ==id);
            _context.Remove(userpatient);
            await _userhelper.DeleteUserAsync(user);            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
