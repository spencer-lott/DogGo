using DogGo.Models;
using DogGo.Models.ViewModels;
using DogGo.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DogGo.Controllers
{
    public class WalksController : Controller
    {
        private readonly IWalkerRepository _walkerRepo;
        private readonly IWalksRepository _walksRepo;
        private readonly IDogRepository _dogRepo;
        public WalksController(
            IWalkerRepository walkerRepository,
            IWalksRepository walksRepository,
            IDogRepository dogRepository)
        {
            _walkerRepo = walkerRepository;
            _walksRepo = walksRepository;
            _dogRepo = dogRepository;
        }

        // GET: WalksController
        public ActionResult Index()
        {
            List<Walks> walks = _walksRepo.GetAllWalks();

            return View(walks);
        }

        // GET: WalksController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: WalksController/Create
        public ActionResult Create()
        {
            List<Dog> dogs = _dogRepo.GetAllDogs();
            List<Walker> walker = _walkerRepo.GetAllWalkers();

            WalkerFormViewModel vm = new WalkerFormViewModel()
            {
                Walkers = walker,
                Dogs = dogs
            };

            return View(vm);
        }

        // POST: WalksController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WalkerFormViewModel vm)
        {
           
            try
            {
                //We are looping through a list of the dogIds and grabbing each one that is selected. Then we add it to the walk
                foreach(int id in vm.DogIds) {
                    vm.Walk.DogId = id;
                _walksRepo.AddWalks(vm.Walk);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(vm);
            }
        }

        // GET: WalksController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: WalksController/Edit/5
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

        // GET: WalksController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: WalksController/Delete/5
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
    }
}
