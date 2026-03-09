using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalovetAPI.Data;
using SalovetAPI.Models;

namespace SalovetAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly SalovetDbContext _context;

        public UsuariosController(SalovetDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene la lista completa de usuarios registrados en el sistema,
        /// incluyendo los datos del cliente asociado a cada uno.
        /// </summary>
        // GET: api/usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios
                .Include(u => u.Cliente)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene un usuario específico por su ID,
        /// incluyendo los datos del cliente asociado.
        /// Devuelve 404 si el usuario no existe.
        /// </summary>
        // GET: api/usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Cliente)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);

            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado" });

            return usuario;
        }

        /// <summary>
        /// Obtiene un usuario específico por su nombre de usuario (username),
        /// incluyendo los datos del cliente asociado.
        /// Devuelve 404 si el username no existe.
        /// </summary>
        // GET: api/usuarios/username/juan123
        [HttpGet("username/{username}")]
        public async Task<ActionResult<Usuario>> GetUsuarioByUsername(string username)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Cliente)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado" });

            return usuario;
        }

        /// <summary>
        /// Crea un nuevo usuario en el sistema.
        /// Verifica previamente que el username no esté ya en uso.
        /// Devuelve 409 Conflict si el username ya existe,
        /// o 201 Created con los datos del usuario creado.
        /// </summary>
        // POST: api/usuarios
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            // Verificar que el username no exista
            if (await _context.Usuarios.AnyAsync(u => u.Username == usuario.Username))
                return Conflict(new { mensaje = "El username ya existe" });

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.IdUsuario }, usuario);
        }

        /// <summary>
        /// Actualiza los datos de un usuario existente identificado por su ID.
        /// Verifica que el ID de la URL coincida con el del cuerpo de la petición.
        /// Devuelve 400 si los IDs no coinciden, 404 si el usuario no existe,
        /// o 204 NoContent si la actualización fue exitosa.
        /// </summary>
        // PUT: api/usuarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.IdUsuario)
                return BadRequest(new { mensaje = "ID no coincide" });

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                    return NotFound(new { mensaje = "Usuario no encontrado" });
                throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Elimina un usuario del sistema por su ID.
        /// Devuelve 404 si el usuario no existe,
        /// o 204 NoContent si fue eliminado correctamente.
        /// </summary>
        // DELETE: api/usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado" });

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Método auxiliar privado que comprueba si existe un usuario
        /// con el ID indicado.Usado internamente para validaciones.
        /// </summary>
        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.IdUsuario == id);
        }
    }
}