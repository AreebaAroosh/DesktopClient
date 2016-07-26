#pragma once

#include "d3d9.h"
#include "Vertex.h"
#include "atlbase.h"
#include "Macros.h"
#include <windows.h>
#include "Overlay.h"

#if defined(LIBRARY_EXPORT)
#   define EXPORTLIB  __declspec(dllexport)
#else 
#   define EXPORTLIB  __declspec(dllimport)
#endif 

#ifdef __cplusplus

enum FillMode
{
        KeepAspectRatio = 0,
        Fill = 1
};

struct IDisplable
{
	virtual HRESULT Initialize(HWND hDisplayWindow) PURE;
	virtual HRESULT CheckFormatConversion(D3DFORMAT format) PURE;
	virtual HRESULT CreateVideoSurface(int width, int height, D3DFORMAT format) PURE;
	virtual HRESULT Display(BYTE* pYplane, BYTE* pVplane, BYTE* pUplane) PURE;
	virtual HRESULT CaptureDisplayFrame(BYTE* pBuffer, INT* width, INT* height, INT* stride) PURE;
	virtual HRESULT CaptureVideoFrame(BYTE* pBuffer, INT* width, INT* height, INT* stride) PURE;
	virtual void SetDisplayMode(FillMode mode) PURE;
    virtual FillMode GetDisplayMode() PURE;
};

struct IShaderable
{
	virtual HRESULT SetPixelShader(LPCWSTR pPixelShaderName, LPCSTR entryPoint, LPCSTR shaderModel, LPSTR* ppError) PURE;
	virtual HRESULT SetPixelShader(DWORD* buffer) PURE;
	virtual HRESULT SetPixelShaderConstant(LPCSTR name, LPVOID value, UINT size) PURE;

	virtual HRESULT SetVertexShader(LPCWSTR pVertexShaderName, LPCSTR entryPoint, LPCSTR shaderModel, LPSTR* ppError) PURE;
	virtual HRESULT SetVertexShader(DWORD* buffer) PURE;
	virtual HRESULT SetVertexShaderConstant(LPCSTR name, LPVOID value, UINT size) PURE;
	virtual HRESULT ApplyWorldViewProj(LPCSTR matrixName) PURE;

	virtual HRESULT SetVertexShaderMatrix(D3DXMATRIX* matrix, LPCSTR name) PURE;
	virtual HRESULT SetPixelShaderMatrix(D3DXMATRIX* matrix, LPCSTR name) PURE;
	virtual HRESULT SetVertexShaderVector(D3DXVECTOR4* vector, LPCSTR name) PURE;
	virtual HRESULT SetPixelShaderVector(D3DXVECTOR4* vector, LPCSTR name) PURE;

	virtual HRESULT ClearPixelShader() PURE;
	virtual HRESULT ClearVertexShader() PURE;
};

struct IOverlable
{
	virtual HRESULT DrawLine(SHORT key, POINT p1, POINT p2, FLOAT width, D3DCOLOR color, BYTE opacity) PURE;
	virtual HRESULT DrawRectangle(SHORT key, RECT rectangle, FLOAT width, D3DCOLOR color, BYTE opacity) PURE;
	virtual HRESULT DrawPolygon(SHORT key, POINT* points, INT pointsLen, FLOAT width, D3DCOLOR color, BYTE opacity ) PURE;
	virtual HRESULT DrawText(SHORT key, LPCWSTR text, RECT pos, INT size, D3DCOLOR color, LPCWSTR font, BYTE opacity) PURE;
	virtual HRESULT DrawBitmap(SHORT key, POINT position, INT width, INT height, BYTE* pPixelData, BYTE opacity) PURE;
	virtual HRESULT RemoveOverlay(SHORT key) PURE;
	virtual HRESULT RemoveAllOverlays() PURE;
};

struct IRenderable : public IDisplable, public IShaderable, public IOverlable
{
};

extern "C" EXPORTLIB IRenderable* APIENTRY GetImplementation();

#endif // __cplusplus