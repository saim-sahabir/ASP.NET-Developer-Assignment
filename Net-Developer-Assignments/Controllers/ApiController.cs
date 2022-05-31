using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Net_Developer_Assignments.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
       
        private readonly ILogger<ApiController> _logger;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ApiController(ILogger<ApiController> logger, IHostingEnvironment hostingEnvironment)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost ("/post")] 
        public IActionResult Post([FromHeader] int pageSize ,  [FromBody] List<string> words )
        {
            
            var rootPath = _hostingEnvironment.ContentRootPath;
            var fullPath = Path.Combine(rootPath, "Data/Data.json");

            var result = new List<string>();
            foreach (var word in words)
            {
                if (word.Length <= pageSize)
                {
                    result.Add(word);
                }else if(word.Length > pageSize)
                { 
                    var lines =  Regex.Matches(word, ".{1," + pageSize + "}").Cast<Match>().Select(m => m.Value).ToArray();
                  foreach (var line in lines)
                  {
                      result.Add(line);
                  }
                  //result.Add(Regex.Replace(word, "(.{" + pageSize + "})", "$1" + Environment.NewLine));
               
                }
               
            }
            
            var wordToJson = JsonSerializer.Serialize(result);
             System.IO.File.WriteAllText(fullPath, wordToJson);
             return StatusCode(201);
        }
        
        
        [HttpGet("/get")]
        
        public IActionResult Get()
        { 
            var rootPath = _hostingEnvironment.ContentRootPath;
            var fullPath = Path.Combine(rootPath, "Data/Data.json");
            var json = System.IO.File.ReadAllText(fullPath);
            
          return StatusCode(200, json);
        }
    }
}