using JsonBridgeEF.Seeding.SourceJson.Models;

namespace JsonBridgeEF.Seeding.SourceJson.Helpers
{
    /// <summary>
    /// Manages a collection of independent blocks.
    /// </summary>
    internal class JsonIndepBlockRegistry
    {
        private readonly Dictionary<string, JsonIndepBlockInfo> _blocks;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonIndepBlockRegistry"/> class.
        /// </summary>
        public JsonIndepBlockRegistry()
        {
            _blocks = [];
        }

        /// <summary>
        /// Adds a new independent block if it does not already exist with a consistent name and key field.
        /// If the block exists, the method checks for key field consistency.
        /// </summary>
        /// <param name="name">The name of the block.</param>
        /// <param name="keyField">The key field associated with the block.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if a block with the same name exists but has a different key field.
        /// </exception>
        public void AddBlock(string name, string keyField)
        {
            if (_blocks.TryGetValue(name, out var existingBlock))
            {
                if (existingBlock.KeyField != keyField)
                {
                    throw new InvalidOperationException($"Conflict in key field for block '{name}'. Found '{keyField}', expected '{existingBlock.KeyField}'.");
                }
            }
            else
            {
                _blocks[name] = JsonIndepBlockInfo.Create(name, keyField);
            }
        }

        /// <summary>
        /// Checks whether an independent block exists in the registry.
        /// </summary>
        /// <param name="name">The name of the block.</param>
        /// <returns><c>true</c> if the block exists; otherwise, <c>false</c>.</returns>
        public bool ContainsBlock(string name)
        {
            return _blocks.ContainsKey(name);
        }

        /// <summary>
        /// Gets the key field associated with a given independent block.
        /// </summary>
        /// <param name="name">The name of the block.</param>
        /// <returns>The key field of the block.</returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown if the block with the specified name does not exist in the registry.
        /// </exception>
        public string GetKeyForBlock(string name)
        {
            if (_blocks.TryGetValue(name, out var block))
            {
                return block.KeyField;
            }
            throw new KeyNotFoundException($"Block '{name}' not found in registry.");
        }

        /// <summary>
        /// Retrieves an independent block by its name.
        /// </summary>
        /// <param name="name">The name of the block.</param>
        /// <returns>
        /// An instance of <see cref="JsonIndepBlockInfo"/> if found; otherwise, <c>null</c>.
        /// </returns>
        public JsonIndepBlockInfo? GetBlock(string name)
        {
            return _blocks.TryGetValue(name, out var block) ? block : null;
        }

        /// <summary>
        /// Retrieves all independent blocks stored in the registry.
        /// </summary>
        /// <returns>A list of all <see cref="JsonIndepBlockInfo"/> instances.</returns>
        public List<JsonIndepBlockInfo> GetAllBlocks()
        {
            return [.. _blocks.Values];
        }

        /// <summary>
        /// Gets the total number of independent blocks in the registry.
        /// </summary>
        public int Count => _blocks.Count;

        /// <summary>
        /// Prints all independent blocks stored in the registry.
        /// </summary>
        public void PrintBlocks()
        {
            Console.WriteLine("Independent blocks found:");
            foreach (var block in _blocks.Values)
            {
                Console.WriteLine($"- {block}");
            }
        }
    }
}