namespace LibraryDomain;
// Interfaces in C# - They are a contract for behaviours - they do not define the implementation of the method within
// 
public interface ILendable
{
   // Only method signatures, no bodies, not even access modifiers
   bool CheckOut();
   void Return(); 
}