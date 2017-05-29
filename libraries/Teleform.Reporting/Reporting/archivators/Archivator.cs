using System;

namespace Teleform.IO.Compression
{
    /// <summary>
    /// Представляет алгоритм создания архива.
    /// </summary>
    public abstract class Archivator
    {
        /// <summary>
        /// Создает архив указанной директории.
        /// </summary>
        /// <param name="folder">Директория, подлежащая архивированию.</param>
        /// <param name="archive">Путь к готовому архиву.</param>
        public abstract void Create(string folder, string archive);
    }
}
