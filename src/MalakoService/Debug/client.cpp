#include <iostream>

__declspec(dllimport) int fnmalakoexport(void);
__declspec(dllimport) void MalakoSound(void);

using namespace std;

int main (void) {
	cout << "Numero retornado: " << fnmalakoexport() << endl;
	
	cout << "Tocando o som" << endl;
	MalakoSound();
	
	return 0;
}