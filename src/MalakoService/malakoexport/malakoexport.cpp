// malakoexport.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "malakoexport.h"


// This is an example of an exported variable
MALAKOEXPORT_API int nmalakoexport=0;

// This is an example of an exported function.
MALAKOEXPORT_API int fnmalakoexport(void)
{
	return 42;
}

MALAKOEXPORT_API void MalakoSound(void)
{
	Beep(750, 400);
}

// This is the constructor of a class that has been exported.
// see malakoexport.h for the class definition
Cmalakoexport::Cmalakoexport()
{
	return;
}
