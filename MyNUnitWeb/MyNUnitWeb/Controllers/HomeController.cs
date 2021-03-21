using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyNUnitWeb.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyNUnitWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly Repository _repository;

        public HomeController(IWebHostEnvironment environment, Repository repository)
        {
            _environment = environment;
            _repository = repository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult History()
        {
            return View(_repository.Assemblies.Include(a => a.Tests).ToList());
        }

        [HttpGet("History/Details/{id}")]
        public IActionResult Details(int? id)
        {
            return View(_repository.Assemblies.Include(a => a.Tests).First(a => a.Id == id));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> LoadAssembliesAsync(IFormFile assembly)
        {
            if (!Directory.Exists($"{_environment.WebRootPath}/Files/"))
            {
                Directory.CreateDirectory($"{_environment.WebRootPath}/Files/");
            }
            if (assembly != null)
            {
                using var fileStream = new FileStream($"{_environment.WebRootPath}/Files/{assembly.FileName}", FileMode.Create);
                await assembly.CopyToAsync(fileStream);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RunTestsAsync()
        {
            foreach (var assemblyName in Directory.EnumerateFiles(_environment.WebRootPath + "/Files/"))
            {
                var name = assemblyName.Split("/Files/")[1];
                var testedAssembly = _repository.Assemblies.Include(a => a.Tests).FirstOrDefault(a => a.Name == name);
                if (testedAssembly == null)
                {
                    testedAssembly = (await _repository.AddAsync(new Assembly { Name = name })).Entity;
                    await _repository.SaveChangesAsync();
                }
                
                _repository.RemoveRange(testedAssembly.Tests);
                await _repository.SaveChangesAsync();
                
                var testMethods = await MyNUnit.MyNUnit.Run(assemblyName);
                foreach (var testMethod in testMethods)
                {
                    var testModel = new Test
                    {
                        Name = testMethod.Method.Name,
                        ExecutionStatus = testMethod.ExecutionResult.Status,
                        ExecutionTime = testMethod.ExecutionResult.ExecutionTime,
                        Message = testMethod.ExecutionResult.Message
                    };
                    testedAssembly.Tests.Add(testModel);
                }
                await _repository.SaveChangesAsync();
            }            
            return RedirectToAction("Index");
        }
    }
}
