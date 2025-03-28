#include "pch.h"
#include "App.h"

extern "C"
{
    // Hint that the discrete gpu should be enabled on optimus/enduro systems
    // NVIDIA docs: http://developer.download.nvidia.com/devzone/devcenter/gamegraphics/files/OptimusRenderingPolicies.pdf
    // AMD forum post: http://devgurus.amd.com/thread/169965
    __declspec(dllexport) DWORD NvOptimusEnablement = 0x00000001;
    __declspec(dllexport) int AmdPowerXpressRequestHighPerformance = 1;

    // Agility SDK
    __declspec(dllexport) extern const UINT D3D12SDKVersion = 610;
    __declspec(dllexport) extern const char* D3D12SDKPath = u8".\\D3D12\\";
}

struct RoInitializeWrapper
{
    inline RoInitializeWrapper() { RoInitialize(RO_INIT_MULTITHREADED); }
    inline ~RoInitializeWrapper() { RoUninitialize(); }
};

int CALLBACK wWinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPWSTR lpCmdLine, int nCmdShow)
{
    RoInitializeWrapper roInit;

    Windows::ApplicationModel::Core::CoreApplication::Run(ref new Grapple_Game_Graybox_::App());
    return 0;
}
