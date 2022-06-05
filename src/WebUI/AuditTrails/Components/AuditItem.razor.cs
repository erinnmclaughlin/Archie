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

        private string GetBadgeIcon()
        {
            return AuditType switch
            {
                AuditType.Create => "fa-solid fa-plus",
                AuditType.Update => "fa-solid fa-pencil-alt",
                AuditType.Delete => "fa-solid fa-minus",
                _ => "fa-solid fa-edit"
            };
        }

        private string GetBadgeColor()
        {
            return AuditType switch
            {
                AuditType.Create => "bg-success",
                AuditType.Update => "bg-primary",
                AuditType.Delete => "bg-danger",
                _ => "bg-secondary"
            };
        }

        private string GetIcon()
        {
            return EventType switch
            {
                EventType.CustomerCreated or EventType.CustomerLocationUpdated or EventType.CustomerNameUpdated => "fa-solid fa-building",
                EventType.WorkOrderCreated or EventType.WorkOrderCompleted or EventType.WorkOrderCompleted => "fa-solid fa-file",
                _ => "fa-solid fa-file"
            };
        }
    }
}