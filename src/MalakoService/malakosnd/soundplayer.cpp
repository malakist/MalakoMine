#include <windows.h>

extern "C" void MalakoSound() {
	Beep(750, 300);
}

int main(void) {
	return 0;
}