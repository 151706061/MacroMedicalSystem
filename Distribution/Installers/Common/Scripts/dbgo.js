WScript.Echo("Opening Script...");

// Create the object 
fs = new ActiveXObject("Scripting.FileSystemObject");
f = fs.GetFile(WScript.Arguments(0));

// Open the file 
is = f.OpenAsTextStream( 1, 0 );

WScript.Echo("Connecting...");

var connection = WScript.CreateObject("ADODB.connection");

connection.Provider = "sqloledb";
connection.Properties("Data Source").Value = "++HOST++";
connection.Properties("Initial Catalog").Value = "master";
connection.Open("", "++ADMINID++", "++ADMINPASS++");

WScript.Echo("Connected.");


// start and continue to read until we hit
// a GO statement, comment, or eof, then execute
while( !is.AtEndOfStream )
{   
   var line = trim(is.ReadLine());
   var statement = "";      
   while ( line != "GO" && !is.AtEndOfStream)   
   {   
	   if (line != "" 
		&& line.substring(0, 2) != "/*" 
		&& line.substring(0, 2) != "--")
		{
			statement += line + " ";
		}
	
	line = trim(is.ReadLine());
	}
	if (statement != "")
	{   
		connection.Execute(statement);
	}
}

// Close the stream 
is.Close();

WScript.Echo("Done");

/**
*
* Javascript trim, ltrim, rtrim
* http://www.webtoolkit.info/
*
*
**/

function trim(str, chars) {
    return ltrim(rtrim(str, chars), chars);
}

function ltrim(str, chars) {
    chars = chars || "\\s";
    return str.replace(new RegExp("^[" + chars + "]+", "g"), "");
}

function rtrim(str, chars) {
    chars = chars || "\\s";
    return str.replace(new RegExp("[" + chars + "]+$", "g"), "");
}