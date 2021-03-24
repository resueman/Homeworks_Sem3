using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyNUnitWeb.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyNUnitWeb.Controllers
{
    /// <summary>
    /// Main controller
    /// </summary>
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly Repository _repository;
        private readonly CurrentState _currentState;

        /// <summary>
        /// Creates instance of controller
        /// </summary>
        /// <param name="environment">environment</param>
        /// <param name="repository">repository containing history</param>
        public HomeController(IWebHostEnvironment environment, Repository repository)
        {
            _environment = environment;
            _repository = repository;
            _currentState = new CurrentState(_environment);
        }

        /// <summary>
        /// Main page
        /// </summary>
        public IActionResult Index()
        {
            return View("Index", _currentState);
        }

        /// <summary>
        /// Show history of all testing assemblies
        /// </summary>
        public IActionResult History()
        {
            return View(_repository.Assemblies.Include(a => a.Tests).ToList());
        }

        /// <summary>
        /// Shows details about execution of tests containing in particular assembly
        /// </summary>
        /// <param name="id">id of assembly which details we want to see</param>
        [HttpGet("History/Details/{id}")]
        public IActionResult Details(int? id)
        {
            return View(_repository.Assemblies.Include(a => a.Tests).First(a => a.Id == id));
        }

        /// <summary>
        /// Load assemblies fo testing
        /// </summary>
        /// <param name="assembly">Assembly</param>
        [HttpPost]
        public async Task<IActionResult> LoadAssembliesAsync(IFormFile assembly)
        {
            if (!Directory.Exists($"{_environment.WebRootPath}/Assemblies/"))
            {
                Directory.CreateDirectory($"{_environment.WebRootPath}/Assemblies/");
            }
            if (assembly != null)
            {
                using var fileStream = new FileStream($"{_environment.WebRootPath}/Assemblies/{assembly.FileName}", FileMode.Create);
                await assembly.CopyToAsync(fileStream);
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Deletes loaded assemblies
        /// </summary>
        public IActionResult DeleteCurrentAssemblies()
        {
            var directoryInfo = new DirectoryInfo($"{_environment.WebRootPath}/Assemblies/");
            foreach (var file in directoryInfo.GetFiles())
            {
                file.Delete();
            }
            return RedirectToAction("Index");
        }

        private static string FormatTimeSpan(TimeSpan timeSpan)
            => string.Format("{0:00}:{1:0000}", timeSpan.Seconds, timeSpan.Milliseconds);

        /// <summary>
        /// Starts tests containing in loaded assemblies
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> RunTestsAsync()
        {
            foreach (var assemblyName in Directory.EnumerateFiles($"{_environment.WebRootPath}/Assemblies/"))
            {

                var name = Path.GetFileName(assemblyName);
                var testedAssembly = _repository.Assemblies.Include(a => a.Tests).FirstOrDefault(a => a.Name == name);
                if (testedAssembly == null)
                {
                    testedAssembly = (await _repository.AddAsync(new Assembly { Name = name })).Entity;
                    await _repository.SaveChangesAsync();
                }
                
                _repository.RemoveRange(testedAssembly.Tests);
                await _repository.SaveChangesAsync();
                
                var testMethods = (await MyNUnit.MyNUnit.Run(assemblyName)).SelectMany(t => t.TestMethods).ToList();
                foreach (var testMethod in testMethods)
                {
                    var testModel = new Test
                    {
                        Name = testMethod.Method.Name,
                        ExecutionStatus = testMethod.ExecutionResult.Status,
                        ExecutionTime = FormatTimeSpan(testMethod.ExecutionResult.ExecutionTime),
                        Message = testMethod.ExecutionResult.Message,
                        AssemblyName = name
                    };
                    testedAssembly.Tests.Add(testModel);
                    _currentState.Tests.Add(testModel);
                }
                await _repository.SaveChangesAsync();
            }            
            return View("Index", _currentState);
        }
    }
}
