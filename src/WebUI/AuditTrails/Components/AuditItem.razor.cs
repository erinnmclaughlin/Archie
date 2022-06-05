using Archie.Shared.Audits;
using Microsoft.AspNetCore.Components;

namespace Archie.WebUI.AuditTrails.Components
{
    public partial class AuditItem
    {
        [Parameter, EditorRequired] public AuditType AuditType { get; set; }
        [Parameter, EditorRequired] public string Description { get; set; } = "";
        [Parameter, EditorRequired] public EventType EventType { get; set; }
        [Parameter, EditorRequired] public DateTime Timestamp { get; set; }
        [Parameter, EditorRequired] public string User { get; set; } = "";
        [Parameter] public RenderFragment? ChildContent { get; set; }

        private string GetIcon()
        {
            if (AuditType == AuditType.Create)
            {
                return "text-success " + EventType switch
                {
                    EventType.CustomerCreated => "fa-solid fa-building-circle-check",
                    EventType.WorkOrderCreated => "fa-solid fa-file-circle-plus",
                    _ => "fa-solid fa-plus"
                };
            }

            if (AuditType == AuditType.Update)
            {
                return "text-primary " + EventType switch
                {
                    EventType.CustomerLocationUpdated => "fa-solid fa-building-circle-arrow-right",
                    EventType.CustomerNameUpdated => "fa-solid fa-building-user",
                    _ => "fa-solid fa-pencil-alt"
                };
            }

            if (AuditType == AuditType.Delete)
            {
                return "text-danger " + EventType switch
                {
                    _ => "fa-solid fa-trash-alt"
                };
            }

            return "fa-solid fa-plus text-muted";
        }
    }
}