#include <windows.h>

extern "C" void MalakoSound(void) {
	Beep(750, 300);
}

int main(void) {
	return 0;
}