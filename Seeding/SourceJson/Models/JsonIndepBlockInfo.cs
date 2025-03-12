using System.Text.RegularExpressions;
using JsonBridgeEF.Common;

namespace JsonBridgeEF.Seeding.SourceJson.Models
{
    /// <summary>
    /// Rappresenta un blocco indipendente con nome, campo chiave, foreign key e riferimento al suo schema.
    /// </summary>
    internal class JsonIndepBlockInfo : ModelBase<JsonIndepBlockInfo>
    {
        public int Id { get; set; }  // Identificativo univoco (utile per la persistenza)

        /// <summary>
        /// Nome del blocco, sanitizzato (senza spazi).
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Campo chiave del blocco.
        /// </summary>
        public string KeyField { get; }

        /// <summary>
        /// Nome della foreign key generata: formato "_IB_{Name}_{KeyField}".
        /// </summary>
        public string ForeignKeyName { get; }

        /// <summary>
        /// Chiave esterna che indica lo schema a cui il blocco appartiene.
        /// Ogni blocco ha un solo schema di riferimento.
        /// </summary>
        public int JsonSchemaDefId { get; set; }


        private JsonIndepBlockInfo(string name, string keyField)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Block name cannot be null or empty.", nameof(name));
            if (string.IsNullOrWhiteSpace(keyField))
                throw new ArgumentException("Key field cannot be null or empty.", nameof(keyField));

            Name = Sanitize(name);
            KeyField = Sanitize(keyField);
            ForeignKeyName = ForeignKeyNameGenerator.Generate(Name, KeyField);
        }

        public override string ToString()
        {
            return $"{Name} (Key: {KeyField})";
        }

        public static bool IsForeignKeyFormat(string value)
        {
            return ForeignKeyNameGenerator.IsValidFormat(value);
        }

        public static string Sanitize(string input)
        {
            return input.Replace(" ", "");
        }

        public static JsonIndepBlockInfo Create(string name, string keyField)
        {
            return new JsonIndepBlockInfo(name, keyField);
        }

        public static JsonIndepBlockInfo Create(string foreignKeyName)
        {
            var (name, keyField) = ForeignKeyNameGenerator.ParseForeignKeyName(foreignKeyName);
            return new JsonIndepBlockInfo(name, keyField);
        }

        public static (string name, string keyField) ExtractForeignKeyComponents(string foreignKeyName)
        {
            return ForeignKeyNameGenerator.ParseForeignKeyName(foreignKeyName);
        }

        private static class ForeignKeyNameGenerator
        {
            private static readonly string Pattern = @"^_IB_[a-zA-Z0-9_]+_[a-zA-Z0-9_]+$";

            public static string Generate(string name, string keyField)
            {
                return $"_IB_{Sanitize(name)}_{Sanitize(keyField)}";
            }

            public static bool IsValidFormat(string value)
            {
                if (string.IsNullOrWhiteSpace(value))
                    return false;
                return new Regex(Pattern, RegexOptions.Compiled).IsMatch(value);
            }

            public static (string name, string keyField) ParseForeignKeyName(string foreignKeyName)
            {
                string withoutPrefix = foreignKeyName[4..];
                int underscoreIndex = withoutPrefix.IndexOf('_');
                if (underscoreIndex < 0)
                    throw new ArgumentException("Foreign key format is invalid.", nameof(foreignKeyName));

                string blockName = withoutPrefix[..underscoreIndex];
                string keyField = withoutPrefix[(underscoreIndex + 1)..];

                if (string.IsNullOrWhiteSpace(blockName) || string.IsNullOrWhiteSpace(keyField))
                    throw new ArgumentException("Foreign key does not contain valid block name or key field.", nameof(foreignKeyName));

                return (blockName, keyField);
            }
        }
    }
}