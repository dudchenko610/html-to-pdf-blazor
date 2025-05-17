using BlazorComponentHeap.Core.Services;
using BlazorComponentHeap.Modal.Services;
using BlazorComponentHeap.Modal.Services.Interfaces;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using HtmlToPdf.WebWasm;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var services = builder.Services;

services.AddScoped<IJsUtilsService, JsUtilsService>();
services.AddScoped<IModalService, ModalService>();

await builder.Build().RunAsync();