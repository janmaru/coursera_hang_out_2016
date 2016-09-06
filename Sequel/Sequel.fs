namespace Mahamudra.System.Data.Sql  
open System.Data // IDbConnection - IDbCommand
open System.Data.Common //  DbParameter
open System.Data.SQLite
open System.Data.SqlClient
open System.Data.SqlServerCe
open Oracle.ManagedDataAccess.Client
open MySql.Data.MySqlClient
open Npgsql
open Mahamudra.System.Railway

//#region DbFactory methods
// connection DbFactory
//Methods with curried arguments cannot be overloaded. 
type DbFactory =  
    static member createCn(provider:SQLProvider) (connectionString:string) =
        let cn:IDbConnection =
            match provider with
            | SQLite -> new SQLiteConnection(connectionString) :> IDbConnection
            | SQLCe  -> new SqlCeConnection(connectionString) :> IDbConnection
            | Oracle -> new OracleConnection(connectionString) :> IDbConnection
            | MySQL -> new MySqlConnection(connectionString) :> IDbConnection
            | PostgreSQL -> new NpgsqlConnection(connectionString) :> IDbConnection
            | _-> new SqlConnection(connectionString) :> IDbConnection 
        cn

    static member createCmd (provider:SQLProvider) (cn:IDbConnection) (sql:string) =
        let cmd:IDbCommand = 
            match provider with
            | SQLite -> new SQLiteCommand(sql,cn:?>SQLiteConnection):> IDbCommand
            | SQLCe -> new SqlCeCommand(sql,cn:?>SqlCeConnection):> IDbCommand
            | Oracle -> new OracleCommand(sql,cn:?>OracleConnection) :> IDbCommand
            | MySQL -> new MySqlCommand(sql,cn:?>MySqlConnection) :> IDbCommand
            | PostgreSQL -> new NpgsqlCommand(sql,cn:?>NpgsqlConnection) :> IDbCommand
            | _-> new SqlCommand(sql,cn:?>SqlConnection):> IDbCommand 
        cmd

    static member createPmt (provider:SQLProvider) (parameter:string*'paramValue) =
        let prm:DbParameter = 
            match provider with
            | SQLite -> new SQLiteParameter(fst parameter, snd parameter):> DbParameter
            | SQLCe -> new SqlCeParameter(fst parameter, snd parameter):> DbParameter
            | Oracle -> new OracleParameter(fst parameter, snd parameter) :> DbParameter
            | MySQL -> new MySqlParameter(fst parameter, snd parameter) :> DbParameter
            | PostgreSQL -> new NpgsqlParameter(fst parameter, snd parameter) :> DbParameter
            | _-> new SqlParameter(fst parameter, snd parameter):> DbParameter 
        prm

type Sequel =  
    static member uQuery  (provider:SQLProvider)
                            (connectionString: string) 
                            (commandType:CommandType) 
                            (parameters:seq<string*'paramValue>) 
                            (bind:IDataReader->'Result) 
                            (sql: string)  = 
                seq { 
                    use cn = DbFactory.createCn provider connectionString 
                    use cmd = DbFactory.createCmd provider cn sql  
                    cmd.CommandType<-commandType 
                    parameters 
                            |> Seq.iter (fun p-> cmd.Parameters.Add(DbFactory.createPmt provider p) |> ignore)

                    cn.Open()
                    use reader = cmd.ExecuteReader()
                    while reader.Read() do
                        yield reader |> bind
                }

    static member query (provider:SQLProvider)
                        (connectionString: string) 
                        (commandType:CommandType) 
                        (parameters:seq<string*'paramValue>) 
                        (bind:IDataReader->'Result) 
                        (sql: string)  = 
                try
                    let someValues = [
                        use cn = DbFactory.createCn provider connectionString 
                        use cmd = DbFactory.createCmd provider cn sql  
                        cmd.CommandType<-commandType 
                        parameters 
                                |> Seq.iter (fun p-> cmd.Parameters.Add(DbFactory.createPmt provider p) |> ignore)

                        cn.Open()
                        use reader = cmd.ExecuteReader()
                        while reader.Read() do
                            yield reader |> bind
                        ]
                    Success someValues
                with
                    | ex -> Failure ex.Message
 
     //execute all CRUD mixed commands; useful for getting the autoincremented id
     static member nonQuery (provider:SQLProvider)
                            (connectionString: string) 
                            (commandType:CommandType) 
                            (parameters:seq<string*'paramValue>) 
                            (sql: string)  = 
                    try
                        use cn = DbFactory.createCn provider connectionString 
                        use cmd = DbFactory.createCmd provider cn sql  
                        cmd.CommandType<-commandType 
                        parameters 
                                |> Seq.iter (fun p-> cmd.Parameters.Add(DbFactory.createPmt provider p) |> ignore)

                        cn.Open()
                        let recordsAffected = cmd.ExecuteNonQuery()
                        Success recordsAffected
                    with
                        | ex -> Failure ex.Message
                             
    //execute all CRUD mixed commands; useful for getting the autoincremented id
     static member mixedQuery (provider:SQLProvider)
                              (connectionString: string) 
                              (commandType:CommandType) 
                              (parameters:seq<string*'paramValue>) 
                              (sql: string)  = 
                    try
                        use cn = DbFactory.createCn provider connectionString 
                        use cmd = DbFactory.createCmd provider cn sql  
                        cmd.CommandType<-commandType 
                        parameters 
                                |> Seq.iter (fun p-> cmd.Parameters.Add(DbFactory.createPmt provider p) |> ignore)

                        cn.Open()
                        let value = cmd.ExecuteScalar()
                        Success value
                    with
                        | ex -> Failure ex.Message

module Client =

///*******
//A sequence is a logical series of elements all of one type. 
//Sequences are particularly useful when you have a large, ordered collection of data but do not necessarily expect to use all the elements. 
//Individual sequence elements are computed only as required, so a sequence can provide better performance than a list in situations in which not all the elements are used.
///*******

//query some records, input tuple, no Result output but a simple sequence
    let uQuery  (provider:SQLProvider)
                (connectionString: string) 
                (commandType:CommandType) 
                (parameters:seq<string*'paramValue>) 
                (bind:IDataReader->'Result) 
                (sql: string)  = 
                seq { 
                    use cn = DbFactory.createCn provider connectionString 
                    use cmd = DbFactory.createCmd provider cn sql  
                    cmd.CommandType<-commandType 
                    parameters 
                            |> Seq.iter (fun p-> cmd.Parameters.Add(DbFactory.createPmt provider p) |> ignore)

                    cn.Open()
                    use reader = cmd.ExecuteReader()
                    while reader.Read() do
                        yield reader |> bind
                }
