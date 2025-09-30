using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientReg.Api.Data;
using PatientReg.Api.Models;

namespace PatientReg.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController(AppDbContext db) : ControllerBase
{
    // Create
    [HttpPost]
    public async Task<ActionResult<Patient>> Create([FromBody] Patient patient)
    {
        // Basic server-side validations
        if (patient.Gender is not ("Male" or "Female" or "Other"))
            return BadRequest(new { error = "gender must be Male, Female, or Other" });

        db.Patients.Add(patient);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
    }

    // Read (paged + search)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Patient>>> List([FromQuery] string? q = null, [FromQuery] int limit = 25, [FromQuery] int offset = 0)
    {
        var query = db.Patients.AsNoTracking().OrderByDescending(p => p.CreatedAt).AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var like = $"%{q}%";
            query = query.Where(p =>
                EF.Functions.Like(p.FirstName, like) ||
                EF.Functions.Like(p.LastName, like) ||
                EF.Functions.Like(p.Phone, like) ||
                EF.Functions.Like(p.Email!, like));
        }

        var result = await query.Skip(offset).Take(limit).ToListAsync();
        return Ok(result);
    }

    // Read single
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Patient>> GetById(int id)
    {
        var patient = await db.Patients.FindAsync(id);
        return patient is null ? NotFound() : Ok(patient);
    }

    // Update
    [HttpPut("{id:int}")]
    public async Task<ActionResult<Patient>> Update(int id, [FromBody] Patient updated)
    {
        var patient = await db.Patients.FindAsync(id);
        if (patient is null) return NotFound();

        // Map fields (avoid overposting by being explicit)
        patient.FirstName = updated.FirstName;
        patient.LastName  = updated.LastName;
        patient.Dob       = updated.Dob;
        patient.Gender    = updated.Gender;
        patient.Phone     = updated.Phone;
        patient.Email     = updated.Email;
        patient.AddressLine1 = updated.AddressLine1;
        patient.AddressLine2 = updated.AddressLine2;
        patient.City      = updated.City;
        patient.State     = updated.State;
        patient.PostalCode = updated.PostalCode;
        patient.InsuranceProvider = updated.InsuranceProvider;
        patient.InsuranceNumber   = updated.InsuranceNumber;
        patient.EmergencyContactName  = updated.EmergencyContactName;
        patient.EmergencyContactPhone = updated.EmergencyContactPhone;

        await db.SaveChangesAsync();
        return Ok(patient);
    }

    // Delete
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var patient = await db.Patients.FindAsync(id);
        if (patient is null) return NotFound();

        db.Patients.Remove(patient);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
