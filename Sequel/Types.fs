//As with other naming guidelines, the goal when naming namespaces is creating sufficient clarity for the programmer using the framework to immediately know what the content of the namespace is likely to be. The following template specifies the general rule for naming namespaces:
//<Company>.(<Product>|<Technology>)[.<Feature>][.<Subnamespace>]

namespace Mahamudra.System.Data.Sql

// "types-only" module
[<AutoOpen>]
module DomainTypes =  
    //we define a type provider of some kind
    type SQLProvider = MSSQLServer | SQLite| SQLCe | Oracle | MySQL |PostgreSQL

