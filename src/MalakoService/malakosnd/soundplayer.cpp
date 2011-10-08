#include <windows.h>

void __declspec(dllexport) PlaySound() {
	Beep(750, 300);
}