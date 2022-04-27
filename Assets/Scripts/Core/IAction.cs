namespace RPG.Core
{
    // GR: The interface defines a "contract" specifying methods or properties that use it that they HAVE TO HAVE.
    // You also CANNOT do any implementation of anything obviously, as that will happen within the class using the "contract" / interface.
    public interface IAction 
    {
        void Cancel(); // GR: NB! Notice, don't need to specify it as public because anything in an interface is always public.
    }
}