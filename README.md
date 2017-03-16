![logo](http://oi65.tinypic.com/2ibivxf.jpg)
# Library Role
Brick Wizard is C# class library mainly  targeted for ASP.NET MVC apps with a simple and pragmatic approach
to deal with wise sequential step Forms that may require flow control  between steps. 
Rather than a complex implementation, Brick wizard rely on a set of conventions that allow you to make simple controllers
 and avoid extra complexity on your views.

# Core brick wizard engine Concepts
## The Library have 5 main concepts (Classes): 
- Wizard 
- Map 
- Route 
- Step 
- TriggerPoint 

Putting things simple.... 

The Route class represent a unique path that the wizard can run from end to end. This path is represented by a collection of steps.
The Wizard Map represent all unique routes available for the wizard to flow.  In essence class Map is a set of routes  and each route is a set of steps (class Steps).
On top of this, the TriggerPoint class represent a set of specific steps in Map where routes intercept and the wizard can shift to another route.

For each wizard type you want to create, you need to define one class that inherits from BrickWizard.Wizard abstract class and define the Model that the wizard will deal with.  
The Wizard abstract class provide you all implementation for except a few abstract methods/Properties that you need to override. 
This abstract concepts will be specific of your own wizard . So...you need to do it by yourself off course :p  

# Credits
- wizard face from logo downloaded at [game-icons.net] (http://game-icons.net/delapouite/originals/wizard-face.html) by [Delapouite] (http://delapouite.com) 


