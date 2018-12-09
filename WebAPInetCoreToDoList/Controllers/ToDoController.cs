using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using WebAPInetCoreToDoList.Models;


namespace WebAPInetCoreToDoList.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        private ApplicationContext context { get; set; }

        public ToDoController(ApplicationContext context)
        {
            this.context = context;
        }

        private bool isNotEmptyHeader()
        {
            if (Request.Headers["userId"] != StringValues.Empty && Request.Headers["userId"].ToString() != String.Empty)
            {
                return true;
            }
            return false;
        }

        // Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TodoItem item)
        {
            if (item == null)
            {
                return BadRequest();
            }

            if (isNotEmptyHeader())
            {
                int UserId = Convert.ToInt32(Request.Headers["userId"]);

                User user = await context.Users.FindAsync(UserId);

                if (user != null)
                {

                    item.UserId = user.Id;
                    context.TodoItems.Add(item);
                    await context.SaveChangesAsync();
                }
                else
                {
                    return Unauthorized();
                }

            }

            return StatusCode(201);
        }

        // Read
        [HttpGet]
        public ActionResult<List<TodoItem>> GetAll()
        {
            if (isNotEmptyHeader())
            {
                int userId = Convert.ToInt32(Request.Headers["userId"]);

                // include role to not null
                User user = context.Users.Include(u => u.TodoItems).Where(u => u.Id == userId).FirstOrDefault();
                
                if (user != null)
                {
                    var items = user.TodoItems;

                    string json = JsonConvert.SerializeObject(items, Formatting.Indented,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        }
                    );

                    return Content(json);
                }
            }

            return Unauthorized();
        }

        // Update
        [HttpPatch]
        public async Task<IActionResult> Update([FromBody] TodoItem item, string id)
        {
            if (item == null)
            {
                return BadRequest();
            }

            if (isNotEmptyHeader())
            {
                int ItemId = Convert.ToInt32(id);

                var todo = await context.TodoItems.FindAsync(ItemId);

                if (todo == null)
                {
                    return NotFound();
                }

                todo.Name = item.Name;
                todo.IsComplete = item.IsComplete;

                context.TodoItems.Update(todo);
                await context.SaveChangesAsync();

                return new NoContentResult();

            }

            return Unauthorized();
        }


        // Delete
        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            if (isNotEmptyHeader())
            {
                var todo = await context.TodoItems.FindAsync(Convert.ToInt32(id));
                if (todo == null)
                {
                    return NotFound();
                }

                context.TodoItems.Remove(todo);
                await context.SaveChangesAsync();

                return new NoContentResult();

            }
            return Unauthorized();
        }




        [HttpGet("admin")]
        public ActionResult<List<TodoItem>> GetAllAdmin()
        {
            if (isNotEmptyHeader())
            {
                int userId = Convert.ToInt32(Request.Headers["userId"]);

                User user = context.Users.Include(u => u.TodoItems).Where(u => u.Id == userId).FirstOrDefault();

                if (user.RoleId != 1) return StatusCode(403);

                string json = JsonConvert.SerializeObject(context.TodoItems.ToList(), Formatting.Indented,
                    new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    }
                );

                return Content(json);
            }

            return Unauthorized();
        }



        [HttpPatch("SetStatus")]
        public async System.Threading.Tasks.Task<IActionResult> ChangeStatus(bool newStatus, string id)
        {
            if (isNotEmptyHeader())
            {
                int ItemId = Convert.ToInt32(id);

                var todo = await context.TodoItems.FindAsync(ItemId);

                if (todo == null)
                {
                    return NotFound();
                }

                todo.IsComplete = newStatus;

                context.TodoItems.Update(todo);
                await context.SaveChangesAsync();

                return new NoContentResult();

            }

            return Unauthorized();
        }


        [HttpGet("done")]
        public ActionResult<List<TodoItem>> GetAllDone()
        {
            if (isNotEmptyHeader())
            {

                int userId = Convert.ToInt32(Request.Headers["userId"]);

                User user = context.Users.Include(u => u.TodoItems).Where(u => u.Id == userId).FirstOrDefault();
                
                string json = JsonConvert.SerializeObject(user.TodoItems.Where(t => t.IsComplete == true).Select(todo => todo.Name).ToList(), Formatting.Indented,
                    new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    }
                );

                return Content(json);
            }

            return Unauthorized();
        }
    }
}