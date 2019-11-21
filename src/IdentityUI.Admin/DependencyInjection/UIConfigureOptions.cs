using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.DependencyInjection
{
    //https://stackoverflow.com/questions/51610513/can-razor-class-library-pack-static-files-js-css-etc-too

    public class UIConfigureOptions : IPostConfigureOptions<StaticFileOptions>
    {
#if NET_CORE2
        private readonly IHostingEnvironment _environment;

        public UIConfigureOptions(IHostingEnvironment hostingEnvironment)
        {
            _environment = hostingEnvironment;
        }
#endif

#if NET_CORE3
        private readonly IWebHostEnvironment _environment;

        public UIConfigureOptions(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        
#endif

        public void PostConfigure(string name, StaticFileOptions options)
        {
            name = name ?? throw new ArgumentNullException(nameof(name));
            options = options ?? throw new ArgumentNullException(nameof(options));

            // Basic initialization in case the options weren't initialized by any other component
            options.ContentTypeProvider = options.ContentTypeProvider ?? new FileExtensionContentTypeProvider();
            if (options.FileProvider == null && _environment.WebRootFileProvider == null)
            {
                throw new InvalidOperationException("Missing FileProvider.");
            }

            options.FileProvider = options.FileProvider ?? _environment.WebRootFileProvider;

            var basePath = "www";

            var filesProvider = new ManifestEmbeddedFileProvider(GetType().Assembly, basePath);
            options.FileProvider = new CompositeFileProvider(options.FileProvider, filesProvider);
        }
    }
}
