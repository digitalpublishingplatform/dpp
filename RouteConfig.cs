using DigitalPublishingPlatform.Helpers;
using Orchard.Mvc.Routes;
using Orchard.WebApi.Routes;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace DigitalPublishingPlatform {

    public class RouteConfig : IHttpRouteProvider {
        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {
                #region Api routing
                new RouteDescriptor {
                    Priority = 200,
                    Route = new Route("help",
                    new RouteValueDictionary{
                        {"area", Constants.Area},
                        {"controller",  "ApiHelpController"},
                        {"action", "Index"}                       
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary{{"area", Constants.Area}},
                    new MvcRouteHandler())                                            
                },                
                new HttpRouteDescriptor {
                    Name = "ControllerOnly",
                    RouteTemplate = "api/Publication",                    
                    Defaults = new {
                        area = Constants.Area,
                        controller = "ApiPublication"
                        
                    }
                },
                new HttpRouteDescriptor {
                    Name = "ControllerAndId",
                    RouteTemplate = "api/Publication/{id}",
                    Defaults = new {
                        area = Constants.Area,
                        controller = "ApiPublication"
                    },
                    Constraints = new {id = @"^\d+s"}
                },
                new HttpRouteDescriptor {
                    Name = "ControllerAndAction",
                    RouteTemplate = "api/Publication/{action}/{id}",
                    Priority = 200,
                    Defaults = new {
                        area = Constants.Area,
                        controller = "ApiPublication",
                        id = RouteParameter.Optional
                    }
                }, 
	            #endregion
                
                #region VideoPicker routing
		        new RouteDescriptor {
                    Priority = 200,
                    Route = new Route("Admin/DigitalPublishingPlatform/VideoPicker/{action}/{id}",
                    new RouteValueDictionary{
                        {"area", Constants.Area},
                        {"controller",  "VideoPicker"},
                        {"action", "Index"},
                        {"id", RouteParameter.Optional}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary{{"area", Constants.Area}},
                    new MvcRouteHandler())                                            
                }, 
	            #endregion
                             
                #region Publication routing
                new RouteDescriptor {
                    Priority = 200,
                    Route = new Route("Admin/DigitalPublishingPlatform/Publication",
                    new RouteValueDictionary{
                        {"area", Constants.Area},
                        {"controller",  "Publication"},
                        {"action", "Index"}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary{{"area", Constants.Area}},
                    new MvcRouteHandler())                                            
                }, 
                new RouteDescriptor {
                    Priority = 200,
                    Route = new Route("Admin/DigitalPublishingPlatform/Publication/Create",
                    new RouteValueDictionary{
                        {"area", Constants.Area},
                        {"controller",  "Publication"},
                        {"action", "Create"}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary{{"area", Constants.Area}},
                    new MvcRouteHandler())                                            
                }, 
                new RouteDescriptor {
                    Priority = 200,
                    Route = new Route("Admin/DigitalPublishingPlatform/Publication/Edit/{id}",
                    new RouteValueDictionary{
                        {"area", Constants.Area},
                        {"controller",  "Publication"},
                        {"action", "Edit"},
                        {"id", RouteParameter.Optional}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary{{"area", Constants.Area}},
                    new MvcRouteHandler())                                            
                }, 
                new RouteDescriptor {
                    Priority = 200,
                    Route = new Route("Admin/DigitalPublishingPlatform/Publication/Delete/{id}",
                    new RouteValueDictionary{
                        {"area", Constants.Area},
                        {"controller",  "Publication"},
                        {"action", "Delete"},
                        {"id", RouteParameter.Optional}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary{{"area", Constants.Area}},
                    new MvcRouteHandler())                                            
                }, 
	            #endregion         

                #region Issue routing
		        new RouteDescriptor {
                    Priority = 200,
                    Route = new Route("Admin/DigitalPublishingPlatform/Publication/{publicationId}/Issue",
                    new RouteValueDictionary{
                        {"area", Constants.Area},
                        {"controller",  "Issue"},
                        {"action", "Index"},
                        {"publicationId", RouteParameter.Optional}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary{{"area", Constants.Area}},
                    new MvcRouteHandler())                                            
                },
                new RouteDescriptor {
                    Priority = 200,
                    Route = new Route("Admin/DigitalPublishingPlatform/Publication/{publicationId}/Issue/Create",
                    new RouteValueDictionary{
                        {"area", Constants.Area},
                        {"controller",  "Issue"},
                        {"action", "Create"},
                        {"publicationId", RouteParameter.Optional}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary{{"area", Constants.Area}},
                    new MvcRouteHandler())                                            
                },
                new RouteDescriptor {
                    Priority = 200,
                    Route = new Route("Admin/DigitalPublishingPlatform/Publication/{publicationId}/Issue/Edit/{id}",
                    new RouteValueDictionary{
                        {"area", Constants.Area},
                        {"controller",  "Issue"},
                        {"action", "Edit"},
                        {"publicationId", RouteParameter.Optional},
                        {"id", RouteParameter.Optional}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary{{"area", Constants.Area}},
                    new MvcRouteHandler())                                            
                },
                new RouteDescriptor {
                    Priority = 200,
                    Route = new Route("Admin/DigitalPublishingPlatform/Publication/{publicationId}/Issue/Delete/{id}",
                    new RouteValueDictionary{
                        {"area", Constants.Area},
                        {"controller",  "Issue"},
                        {"action", "Delete"},
                        {"publicationId", RouteParameter.Optional},
                        {"id", RouteParameter.Optional}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary{{"area", Constants.Area}},
                    new MvcRouteHandler())                                            
                }, 
                new RouteDescriptor {
                    Priority = 200,
                    Route = new Route("Admin/DigitalPublishingPlatform/Publication/{publicationId}/Issue/IssueStructure/{id}",
                    new RouteValueDictionary{
                        {"area", Constants.Area},
                        {"controller",  "Issue"},
                        {"action", "IssueStructure"},
                        {"id", RouteParameter.Optional}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary{{"area", Constants.Area}},
                    new MvcRouteHandler())                                            
                },
	            #endregion
                
                #region Article routing
		        new RouteDescriptor {
                    Priority = 200,
                    Route = new Route("Admin/DigitalPublishingPlatform/Publication/{publicationId}/Issue/{issueId}/Article",
                    new RouteValueDictionary{
                        {"area", Constants.Area},
                        {"controller",  "Article"},
                        {"action", "Index"},
                        {"publicationId", RouteParameter.Optional},
                        {"issueId", RouteParameter.Optional}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary{{"area", Constants.Area}},
                    new MvcRouteHandler())                                            
                },
                new RouteDescriptor {
                    Priority = 200,
                    Route = new Route("Admin/DigitalPublishingPlatform/Publication/{publicationId}/Issue/{issueId}/Article/Create",
                    new RouteValueDictionary{
                        {"area", Constants.Area},
                        {"controller",  "Article"},
                        {"action", "Create"},
                        {"publicationId", RouteParameter.Optional},
                        {"issueId", RouteParameter.Optional}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary{{"area", Constants.Area}},
                    new MvcRouteHandler())                                            
                },
                new RouteDescriptor {
                    Priority = 200,
                    Route = new Route("Admin/DigitalPublishingPlatform/Publication/{publicationId}/Issue/{issueId}/Article/Edit/{id}",
                    new RouteValueDictionary{
                        {"area", Constants.Area},
                        {"controller",  "Article"},
                        {"action", "Edit"},
                        {"publicationId", RouteParameter.Optional},
                        {"issueId", RouteParameter.Optional},
                        {"id", RouteParameter.Optional}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary{{"area", Constants.Area}},
                    new MvcRouteHandler())                                            
                },
                new RouteDescriptor {
                    Priority = 200,
                    Route = new Route("Admin/DigitalPublishingPlatform/Publication/{publicationId}/Issue/{issueId}/Article/Delete/{id}",
                    new RouteValueDictionary{
                        {"area", Constants.Area},
                        {"controller",  "Article"},
                        {"action", "Delete"},
                        {"publicationId", RouteParameter.Optional},
                        {"issueId", RouteParameter.Optional},
                        {"id", RouteParameter.Optional}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary{{"area", Constants.Area}},
                    new MvcRouteHandler())                                            
                }, 
	            #endregion
                
                #region Category routing
                new RouteDescriptor {
                    Priority = 200,
                    Route = new Route("Admin/DigitalPublishingPlatform/Publication/{publicationId}/Category",
                    new RouteValueDictionary{
                        {"area", Constants.Area},
                        {"controller",  "Category"},
                        {"action", "Index"},
                        {"publicationId", RouteParameter.Optional}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary{{"area", Constants.Area}},
                    new MvcRouteHandler())                                            
                }, 
                new RouteDescriptor {
                    Priority = 200,
                    Route = new Route("Admin/DigitalPublishingPlatform/Publication/{publicationId}/Category/Create",
                    new RouteValueDictionary{
                        {"area", Constants.Area},
                        {"controller",  "Category"},
                        {"action", "Create"},
                        {"publicationId", RouteParameter.Optional}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary{{"area", Constants.Area}},
                    new MvcRouteHandler())                                            
                },
                new RouteDescriptor {
                    Priority = 200,
                    Route = new Route("Admin/DigitalPublishingPlatform/Publication/{publicationId}/Category/Edit/{id}",
                    new RouteValueDictionary{
                        {"area", Constants.Area},
                        {"controller",  "Category"},
                        {"action", "Edit"},
                        {"publicationId", RouteParameter.Optional},
                        {"id", RouteParameter.Optional}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary{{"area", Constants.Area}},
                    new MvcRouteHandler())                                            
                },
                new RouteDescriptor {
                    Priority = 200,
                    Route = new Route("Admin/DigitalPublishingPlatform/Publication/{publicationId}/Category/Delete/{id}",
                    new RouteValueDictionary{
                        {"area", Constants.Area},
                        {"controller",  "Category"},
                        {"action", "Delete"},
                        {"publicationId", RouteParameter.Optional},
                        {"id", RouteParameter.Optional}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary{{"area", Constants.Area}},
                    new MvcRouteHandler())                                            
                },
                new RouteDescriptor {
                    Priority = 200,
                    Route = new Route("Admin/DigitalPublishingPlatform/Publication/{publicationId}/Category/UpdateCategoryOrder/{ids}",
                    new RouteValueDictionary{
                        {"area", Constants.Area},
                        {"controller",  "Category"},
                        {"action", "UpdateCategoryOrder"},
                        {"publicationId", RouteParameter.Optional},
                        {"ids", RouteParameter.Optional}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary{{"area", Constants.Area}},
                    new MvcRouteHandler())                                            
                },
                #endregion   

                #region Others routing
		        new RouteDescriptor {
                            Route = new Route("crossdomain.xml",
                            new RouteValueDictionary{
                                {"area", Constants.Area},
                                {"controller",  "Admin"},
                                {"action", "Crossdomain"}                       
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary{{"area", Constants.Area}},
                            new MvcRouteHandler())                                            
                        }, 
	            #endregion

                #region ImagePicker routing
		        new RouteDescriptor {
                    Priority = 200,
                    Route = new Route("Admin/DigitalPublishingPlatform/ImagePicker/{action}/{id}",
                    new RouteValueDictionary{
                        {"area", Constants.Area},
                        {"controller",  "ImagePicker"},
                        {"action", "Index"},
                        {"id", RouteParameter.Optional}
                    },
                    new RouteValueDictionary(),
                    new RouteValueDictionary{{"area", Constants.Area}},
                    new MvcRouteHandler())                                            
                }, 
	            #endregion
            };
        }

        public void GetRoutes(ICollection<RouteDescriptor> routes) {            
            foreach (var routeDescriptor in GetRoutes().Reverse()) {                
                routes.Add(routeDescriptor);
            }
        }
    }
}
