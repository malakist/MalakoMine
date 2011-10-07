#include <iostream>
#include <cstdio>
#include <windows.h>
#include <shellapi.h>
#include <strsafe.h>

int main(void) {
	std::cout << "Servico de alertas sonoros do MalakoMine" << std::endl;
	
	HICON icon;
	
	NOTIFYICONDATA nid = {};
	nid.cbSize = sizeof(nid);
	// nid.hWnd = hWnd;
	nid.uFlags = NIF_ICON | NIF_TIP | NIF_GUID;
	nid.uID = 0;
	// StringCchCopy(nid.szTip, 40, L"Malako Sound Service");
	&nid.szTip = "Malako Sound Service";
	Shell_NotifyIcon(NIM_ADD, &nid);
	
	std::cout << "Pressione qualquer tecla para encerrar..." << std::endl;
	char c;
	std::cin >> c;
	
	return 0;
}