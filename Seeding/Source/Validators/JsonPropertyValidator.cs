using System.ComponentModel.DataAnnotations;
using JsonBridgeEF.Common.Validators;
using JsonBridgeEF.Seeding.Source.Model.JsonProperties;
using JsonBridgeEF.Seeding.Source.Model.JsonEntities;
using JsonBridgeEF.Shared.EntityModel.Validators;

namespace JsonBridgeEF.Seeding.Source.Validators;

/// <summary>
/// Domain Concept: Validatore concreto per <see cref="JsonProperty"/> che estende <see cref="EntityPropertyValidator{TSelf, TEntity}"/>.
/// </summary>
/// <remarks>
/// <para>Creation Strategy: Estende direttamente la logica base tramite ereditarietà.</para>
/// <para>Constraints: Oltre ai vincoli strutturali ereditati, verifica la lunghezza della descrizione.</para>
/// <para>Relationships: Specializzazione di <see cref="EntityPropertyValidator{TSelf, TEntity}"/> per <see cref="JsonProperty"/>.</para>
/// <para>Usage Notes: Se la descrizione è nulla, viene corretta automaticamente in stringa vuota.</para>
/// </remarks>
internal sealed class JsonPropertyValidator
    : EntityPropertyValidator<JsonProperty, JsonEntity>, IValidateAndFix<JsonProperty>
{
    /// <inheritdoc />
    public void EnsureValid(JsonProperty jsonProperty)
    {
        base.EnsureValid(jsonProperty);
        ValidateDescription(jsonProperty.Description);
    }

    /// <inheritdoc />
    public void Fix(JsonProperty jsonProperty)
    {
        base.Fix(jsonProperty);
        jsonProperty.Description = FixDescription(jsonProperty.Description);
    }

    /// <summary>
    /// Verifica che la descrizione non ecceda la lunghezza massima consentita.
    /// </summary>
    private static void ValidateDescription(string? description)
    {
        if (description != null && description.Length > 500)
            throw new ValidationException("The Description cannot exceed 500 characters.");
    }

    /// <summary>
    /// Restituisce una descrizione non nulla, convertendo <c>null</c> in stringa vuota.
    /// </summary>
    private static string FixDescription(string? description)
    {
        return description ?? string.Empty;
    }
}