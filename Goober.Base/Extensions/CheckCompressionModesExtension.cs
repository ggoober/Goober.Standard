using Microsoft.Extensions.Configuration;

namespace Goober.Base.Extensions
{
    /// <summary>
    /// Возвращает значения из файла конфигурации, позволяющие определить задано ли конфигурацией приложения поведение по компрессии-декомпресии входящих-выходящих сообщений
    /// </summary>
    public static class CheckCompressionModesExtension
    {
        /// <summary>
        /// Возвращает флаг, определяющий необходимо ли осуществить декомпрессию входящего на сервер сообщения клиента
        /// </summary>
        /// <param name="configuration">Конфигурация приложения</param>
        /// <param name="decompressRequestConfigPathKey">Ключ-путь внутри конфигурации приложения (по умолчанию в конфигурации приложения производится поиск секции с ключем-путём "CompressionSettings:DecompressRequest").</param>
        /// <returns>true - если значение проставлено в true, false - если значение проставлено в false, false - если во время чтения настройки возникли проблемы.</returns>
        public static bool NeedToAddRequestDecompression(this IConfiguration configuration, string decompressRequestConfigPathKey = "CompressionSettings:DecompressRequest") => 
            ConfigSettingsExtension.GetSettingValueByKeyOrDefault(configuration, decompressRequestConfigPathKey, false);

        /// <summary>
        /// Возвращает флаг, определяющий необходимо ли сжать ответное сообщение сервера перед отправкой клиенту, в случае если общение происходит по https
        /// </summary>
        /// <param name="configuration">Конфигурация приложения</param>
        /// <param name="responseHttpsCompressionConfigPathKey">Ключ-путь внутри конфигурации приложения (по умолчанию в конфигурации приложения производится поиск секции с ключем-путём "CompressionSettings:CompressHttpsResponse").</param>
        /// <returns>true - если значение проставлено в true, false - если значение проставлено в false, false - если во время чтения настройки возникли проблемы.</returns>
        public static bool NeedToUseResponseHttpsCompression(this IConfiguration configuration, string responseHttpsCompressionConfigPathKey = "CompressionSettings:CompressHttpsResponse") => 
            ConfigSettingsExtension.GetSettingValueByKeyOrDefault(configuration, responseHttpsCompressionConfigPathKey, false);

        /// <summary>
        /// Возвращает флаг, определяющий необходимо ли сжать ответное сообщение сервера перед отправкой клиенту, в случае если общение происходит по http
        /// </summary>
        /// <param name="configuration">Конфигурация приложения</param>
        /// <param name="addResponseCompressionConfigPathKey">Ключ-путь внутри конфигурации приложения (по умолчанию в конфигурации приложения производится поиск секции с ключем-путём "CompressionSettings:CompressResponse").</param>
        /// <returns>true - если значение проставлено в true, false - если значение проставлено в false, false - если во время чтения настройки возникли проблемы.</returns>
        public static bool NeedToAddResponseCompression(this IConfiguration configuration, string addResponseCompressionConfigPathKey = "CompressionSettings:CompressResponse") => 
            ConfigSettingsExtension.GetSettingValueByKeyOrDefault(configuration, addResponseCompressionConfigPathKey, false);
    }
}
