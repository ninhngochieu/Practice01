using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice01.Presentation.Common.EventSources;
[EventSource(Name = "MyCompany-MyApp")]
public class MyAppEventSource : EventSource
{
    [Event(1, Message = "User {0} logged in", Level = EventLevel.Informational)]
    public void UserLoggedIn(string username, string role)
        => WriteEvent(1, username, role);
}
