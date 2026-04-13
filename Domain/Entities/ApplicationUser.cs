namespace Domain.Entities;

// NOTA ARQUITECTURAL — Deuda técnica consciente
// ─────────────────────────────────────────────
// ApplicationUser hereda de IdentityUser (Microsoft.Extensions.Identity.Stores),
// introduciendo una dependencia externa en la capa Domain.
//
// RAZÓN: IdentityUser es clase base concreta requerida por UserManager<T>.
// No existe mecanismo de implementación alternativo sin perder Identity.
//
// ATENUANTE (.NET 10): Microsoft.Extensions.Identity.Stores NO incluye
// EF Core ni middleware HTTP — solo los tipos de identidad.
// La dependencia de EF Core permanece correctamente en Persistence.
//
// ALTERNATIVA CORRECTA (no implementada): Proyecto separado
// "Infrastructure.Identity" que aísle ApplicationUser del Domain.

using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public required string NombreCompleto { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navegación hacia la entidad de dominio
    public Estudiante? Estudiante { get; set; }
}