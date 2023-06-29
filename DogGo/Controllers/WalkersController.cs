using DogGo.Models;
using DogGo.Models.ViewModels;
using DogGo.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DogGo.Controllers
{
    public class WalkersController : Controller
    {

        private readonly IWalkerRepository _walkerRepo;
        private readonly IWalksRepository _walksRepo;
        private readonly IDogRepository _dogRepo;
        private readonly IOwnerRepository _ownerRepo;

        public WalkersController(
            IWalkerRepository walkerRepository, 
            IWalksRepository walksRepository,
            IDogRepository dogRepository,
            IOwnerRepository ownerRepo)
        {
            _walkerRepo = walkerRepository;
            _walksRepo = walksRepository;
            _dogRepo = dogRepository;
            _ownerRepo = ownerRepo;
        }

        // GET: WalkersController
        // GET: Walkers
        public ActionResult Index()
        {

            int ownerId = GetCurrentUserId();
            Owner loggedInOwner = _ownerRepo.GetOwnerById(ownerId);
            List<Walker> walkers = _walkerRepo.GetAllWalkers();

            if (ownerId != 0)
            {
                List<Walker> neighborhoodWalkers = walkers.Where(walker => walker.NeighborhoodId == loggedInOwner.NeighborhoodId).ToList(); 
                return View(neighborhoodWalkers);
            }
            else
            {
            return View(walkers);
            }

        }

        // GET: WalkersController/Details/5
        public ActionResult Details(int id)
        {
            Walker walker = _walkerRepo.GetWalkerById(id);
            List<Walks> walks = _walksRepo.GetWalksByWalkerId(walker.Id);

            WalkerFormViewModel vm = new WalkerFormViewModel()
            {
                Walker = walker,
                Walks = walks,
            };

            return View(vm);
        }

        // GET: WalkersController/Create
        public ActionResult Create()
        {
            return View();
        }


        // POST: WalkersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: WalkersController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: WalkersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: WalkersController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: WalkersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private int GetCurrentUserId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id != null) { 
            return int.Parse(id);
            }
            else { return 0; }
        }


    }
}
