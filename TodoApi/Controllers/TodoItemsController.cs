using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoItemsController(TodoContext context)
        {
            _context = context;
        }

        // GET: TodoItems
        [HttpGet]
        public ActionResult<IEnumerable<TodoItemDTO>> GetTodoItems()
        {
            return BadRequest();
        }

        // GET: TodoItems/mikko
        [HttpGet("{user}")]
        public async Task<ActionResult<TodoItemDTO>> GetTodoItemByUser(string user)
        {
            var todoItems = await _context.TodoItems.ToListAsync();

            var filteredTodoItems = todoItems.Where(s => s.User == user).ToList();

            if (filteredTodoItems == null)
            {
                return NotFound();
            }

            filteredTodoItems.Select(x => ItemToDTO(x)).ToList();

            return Ok(filteredTodoItems);
        }

        // GET: TodoItems/pekka/3
        [HttpGet("{user}/{id:int}")]
        public async Task<ActionResult<TodoItemDTO>> GetTodoItemByUserAndId( string user, int id)
        {
            if (user == null)
            {
                return BadRequest();
            }
            
            var todoItems = await _context.TodoItems.ToListAsync();

            user = user.ToLower();
            var filteredTodoItems = todoItems.Where(s => s.User == user).ToList();
            var finalFilteredTodoItems = filteredTodoItems.Where(s => s.Id == id).ToList();

            if (filteredTodoItems == null)
            {
                return NotFound();
            }

            finalFilteredTodoItems.Select(x => ItemToDTO(x)).ToList();

            return Ok(finalFilteredTodoItems);
        }

        // PUT: TodoItems
        [HttpPut]
        public ActionResult<IEnumerable<TodoItemDTO>> PutTodoItem(TodoItem todoItem)
        {
            return BadRequest();
        }

        // PUT: TodoItems/5
        [HttpPut("{user}/{id:long}")]
        public async Task<IActionResult> PutTodoItemByUser(string user, long id, TodoItem todoItem)
        {
            if (id != todoItem.Id || user != todoItem.User)
            {
                return NotFound();
            }
            
            todoItem.User = user;
            todoItem.Id = id;

            _context.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(ItemToDTO(todoItem));
        }

        // POST: TodoItems
        [HttpPost]
        public ActionResult<TodoItemDTO> PostTodoItem(TodoItem todoItem)
        {
            return BadRequest();
        }

        // POST: TodoItems/liisa
        [HttpPost("{user:alpha}")]
        public async Task<ActionResult<TodoItemDTO>> PostTodoItemByUser([FromRoute] string user, TodoItem todoItem)
        {
            if (user == null)
            {
                return BadRequest();
            }
               
            todoItem.User = user;

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            var result = ItemToDTO(todoItem);

            return CreatedAtAction(nameof(GetTodoItemByUser), new { id = todoItem.Id }, result);
        }

        // DELETE: TodoItems/ville/7
        [HttpDelete("{user}/{id}")]
        public async Task<IActionResult> DeleteTodoItemByUser(long id)
        {
            if (User == null)
            {
                return BadRequest();
            }

            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TodoItemExists(long id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }

        private static TodoItemDTO ItemToDTO(TodoItem todoItem) => new TodoItemDTO
        {
            Id = todoItem.Id,
            Name = todoItem.Name,
            IsComplete = todoItem.IsComplete
        };
    }
}
