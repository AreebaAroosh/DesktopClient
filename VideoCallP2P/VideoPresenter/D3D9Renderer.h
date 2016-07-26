// VideoPresenter.h

#pragma once

#include "D3D9RenderImpl.h"
#include "Format.h"
#include "ShaderCompilationException.h"

using namespace System;
using namespace System::Drawing;
using namespace System::Runtime::InteropServices;
using namespace Microsoft::Xna::Framework;

namespace Taygeta { namespace Core 
{	
	/// <summary>
    /// Direct3D 9 rendering implementation class
    /// </summary>
    public ref class D3D9Renderer
	{
	public:
		
		/// <summary>
		/// Initializes new instance the renderer
		/// </summary>
		/// <param name="hWindow">Handle to the display window</param>
		D3D9Renderer(IntPtr hWindow)
		{
			m_impl = new D3D9RenderImpl();
			HRESULT hr = m_impl->Initialize((HWND)hWindow.ToPointer());
			Marshal::ThrowExceptionForHR(hr);
		}

		
		virtual ~D3D9Renderer()
		{
			delete m_impl;
		}

		/// <summary>
		/// Check if graphics device capable of converting given pixel fromat to adapter's display format
		/// </summary>
		/// <param name="format">Pixel format of video frames</param>
		/// <returns>True if conversion supported, otherwise False</returns>
		bool CheckFormatConversion(PixelFormat format)
		{
			switch(format)
			{
				case PixelFormat::YV12:
					return m_impl->CheckFormatConversion(D3DFMT_YV12) == S_OK;

				case PixelFormat::NV12:
					return m_impl->CheckFormatConversion(D3DFMT_NV12) == S_OK;

				case PixelFormat::YUY2:
					return m_impl->CheckFormatConversion(D3DFMT_YUY2) == S_OK;

				case PixelFormat::UYVY:
					return m_impl->CheckFormatConversion(D3DFMT_UYVY) == S_OK;	

				case PixelFormat::RGB15:
					return m_impl->CheckFormatConversion(D3DFMT_X1R5G5B5) == S_OK;

				case PixelFormat::RGB16:
					return m_impl->CheckFormatConversion(D3DFMT_R5G6B5) == S_OK;

				case PixelFormat::RGB32:
					return m_impl->CheckFormatConversion(D3DFMT_X8R8G8B8) == S_OK;

				case PixelFormat::ARGB32:
					return m_impl->CheckFormatConversion(D3DFMT_A8R8G8B8) == S_OK;

				default:
					throw gcnew ArgumentException("Unknown pixel format", "format");
			}
		}

		/// <summary>
		/// Creates surface for source video frames
		/// </summary>
		/// <param name="width">Width of the video frame</param>
		/// <param name="height">Height of the video frame</param>
		/// <param name="format">Pixel format of the video frame</param>
		void CreateVideoSurface(int width, int height, PixelFormat format)
		{
			D3DFORMAT frm;
			switch(format)
			{
				case PixelFormat::YV12:
					frm = D3DFMT_YV12;
					break;

				case PixelFormat::NV12:
					frm = D3DFMT_NV12;
					break;

				case PixelFormat::YUY2:
					frm = D3DFMT_YUY2;
					break;

				case PixelFormat::UYVY:
					frm = D3DFMT_UYVY;
					break;

				case PixelFormat::RGB15:
					frm = D3DFMT_X1R5G5B5;
					break;

				case PixelFormat::RGB16:
					frm = D3DFMT_R5G6B5;
					break;

				case PixelFormat::RGB32:
					frm = D3DFMT_X8R8G8B8;
					break;

				case PixelFormat::ARGB32:
					frm = D3DFMT_A8R8G8B8;
					break;

				default:
					throw gcnew ArgumentException("Unknown pixel format", "format");
			}

			HRESULT hr = m_impl->CreateVideoSurface(width, height, frm);
			Marshal::ThrowExceptionForHR(hr);
		}

		/// <summary>
		/// Displays pixel data
		/// </summary>
		/// <param name="pYplane">First pixel plane</param>
		/// <param name="pVplane">Second pixel plane, may be NULL</param>
		/// <param name="pUplane">Third pixel plane, may be NULL</param>
		/// <param name="bThrowOnError">Indicates whether to throw exception when display fails</param>
		/// <remarks>When rendering RGB packed pixel data pYplane will hold all the data needed. Similarly when rendering
		/// pixel format of 2 planes, like NV12, First snd Second pointers must point to valid data. Also note that there is no
		/// bound checking for pointer size and it is up to you to correctly allign the pixel data and size.</remarks>
		void Display(IntPtr pYplane, IntPtr pVplane, IntPtr pUplane, bool bThrowOnError)
		{
			HRESULT hr = m_impl->Display((BYTE*)pYplane.ToPointer(), (BYTE*)pVplane.ToPointer(), (BYTE*)pUplane.ToPointer());
			if(bThrowOnError)
			{
				Marshal::ThrowExceptionForHR(hr);
			}
		}

		/// <summary>
		/// Displays pixel data
		/// </summary>
		/// <param name="pYplane">First pixel plane</param>
		/// <param name="pVplane">Second pixel plane, may be NULL</param>
		/// <param name="pUplane">Third pixel plane, may be NULL</param>
		/// <param name="bThrowOnError">Indicates whether to throw exception when display fails</param>
		/// <remarks>When rendering RGB packed pixel data pYplane will hold all the data needed. Similarly when rendering
		/// pixel format of 2 planes, like NV12, First snd Second pointers must point to valid data. Also note that there is no
		/// bound checking for pointer size and it is up to you to correctly allign the pixel data and size.</remarks>
		void Display(Byte* pYplane, Byte* pVplane, Byte* pUplane, bool bThrowOnError)
		{
			HRESULT hr = m_impl->Display((BYTE*)pYplane, (BYTE*)pVplane, (BYTE*)pUplane);
			if(bThrowOnError)
			{
				Marshal::ThrowExceptionForHR(hr);
			}
		}

		/// <summary>
		/// Sets pixel shader from source file.
		/// </summary>
		/// <param name="pixelShaderName">Full path to the pixel shader source file.</param>
		/// <param name="entryPoint">Name of the main method</param>
		/// <param name="modelName">Pixel shader mode</param>
		/// <exception cref="ShaderCompilationException">Excpetion thrown when shader compilation fails</exception>
		void SetPixelShader(String^ pixelShaderName, String^ entryPoint, String^ modelName)
		{
			LPCWSTR pPixel = (LPCWSTR)Marshal::StringToHGlobalUni(pixelShaderName).ToPointer();
			LPCSTR pEntryPoint = (LPCSTR)Marshal::StringToHGlobalAnsi(entryPoint).ToPointer();
			LPCSTR pModelName = (LPCSTR)Marshal::StringToHGlobalAnsi(modelName).ToPointer();

			LPSTR errorMsg = NULL;
			HRESULT hr = m_impl->SetPixelShader(pPixel, pEntryPoint, pModelName, &errorMsg);		
			if(FAILED(hr))
			{
				String^ err = gcnew String(errorMsg);
				delete errorMsg;
				throw gcnew ShaderCompilationException(err);
			}
		}

		/// <summary>
		/// Sets pixel shader from compiled bytecode.
		/// </summary>
		/// <param name="buffer">Buffer containing shader bytecode</param>
		void SetPixelShader(array<Byte>^ buffer)
		{
			pin_ptr<BYTE> pBuffer = &buffer[0];
			HRESULT hr = m_impl->SetPixelShader((DWORD*)pBuffer);
			Marshal::ThrowExceptionForHR(hr);
		}

		/// <summary>
		/// Sets constant value for pixel shader
		/// </summary>
		/// <param name="name">Name of the constant</param>
		/// <param name="value">Value of constant</param>
		/// <remarks>For float values use .0f notation, i.e. 2.0f</remarks>
		void SetPixelShaderConstant(String^ name, Object^ value)
		{
			LPCSTR pName = (LPCSTR)Marshal::StringToHGlobalAnsi(name).ToPointer();

			IntPtr pObj;
			try
			{
				pObj = Marshal::AllocHGlobal(Marshal::SizeOf(value));
				Marshal::StructureToPtr(value, pObj, false);

				HRESULT hr = m_impl->SetPixelShaderConstant(pName, pObj.ToPointer(), Marshal::SizeOf(value));
				Marshal::ThrowExceptionForHR(hr);
			}
			finally
			{
				Marshal::FreeHGlobal(pObj);
			}
		}

		/// <summary>
		/// Sets vertex shader from source file.
		/// </summary>
		/// <param name="vertexShaderName">Full path to the vertex shader source file</param>
		/// <param name="entryPoint">Name of the main method</param>
		/// <param name="modelName">Vertex shader mode</param>
		/// <exception cref="ShaderCompilationException">Excpetion thrown when shader compilation fails</exception>
		void SetVertexShader(String^ vertexShaderName, String^ entryPoint, String^ modelName)
		{
			LPCWSTR pPixel = (LPCWSTR)Marshal::StringToHGlobalUni(vertexShaderName).ToPointer();
			LPCSTR pEntryPoint = (LPCSTR)Marshal::StringToHGlobalAnsi(entryPoint).ToPointer();
			LPCSTR pModelName = (LPCSTR)Marshal::StringToHGlobalAnsi(modelName).ToPointer();

			LPSTR errorMsg = NULL;
			HRESULT hr = m_impl->SetVertexShader(pPixel, pEntryPoint, pModelName, &errorMsg);
			if(FAILED(hr))
			{
				String^ err = gcnew String(errorMsg);
				delete errorMsg;
				throw gcnew ShaderCompilationException(err);
			}
		}

		/// <summary>
		/// Sets vertex shader from compiled bytecode.
		/// </summary>
		/// <param name="buffer">Buffer containing shader bytecode</param>
		void SetVertexShader(array<Byte>^ buffer)
		{
			pin_ptr<BYTE> pBuffer = &buffer[0]; 
			HRESULT hr = m_impl->SetVertexShader((DWORD*)pBuffer);
			Marshal::ThrowExceptionForHR(hr);
		}

		/// <summary>
		/// Sets constant value for vertex shader
		/// </summary>
		/// <param name="name">Name of the constant</param>
		/// <param name="value">Value of constant</param>
		/// <remarks>For float values use .0f notation, i.e. 2.0f</remarks>
		void SetVertexShaderConstant(String^ name, Object^ value)
		{
			LPCSTR pName = (LPCSTR)Marshal::StringToHGlobalAnsi(name).ToPointer();

			IntPtr pObj;
			try
			{
				pObj = Marshal::AllocHGlobal(Marshal::SizeOf(value));
				Marshal::StructureToPtr(value, pObj, false);

				HRESULT hr = m_impl->SetVertexShaderConstant(pName, pObj.ToPointer(), Marshal::SizeOf(value));
				Marshal::ThrowExceptionForHR(hr);
			}
			finally
			{
				Marshal::FreeHGlobal(pObj);
			}
		}

		/// <summary>
		/// Sets matrix variable for vertex shader
		/// </summary>
		/// <param name="matrix">Matrix to set</param>
		/// <param name="name">Name of the matrix</param>
		void SetVertexShaderMatrix(Matrix matrix, String^ name)
		{
			LPCSTR pMatrixName = (LPCSTR)Marshal::StringToHGlobalAnsi(name).ToPointer();

			D3DXMATRIX nativeMatrix(matrix.M11, matrix.M12, matrix.M13, matrix.M14,
									matrix.M21, matrix.M22, matrix.M23, matrix.M24,
									matrix.M31, matrix.M32, matrix.M33, matrix.M34,
									matrix.M41, matrix.M42, matrix.M43, matrix.M44);

			HRESULT hr = m_impl->SetVertexShaderMatrix(&nativeMatrix, pMatrixName);
			Marshal::ThrowExceptionForHR(hr);
		}

		/// <summary>
		/// Sets matrix variable for pixel shader
		/// </summary>
		/// <param name="matrix">Matrix to set</param>
		/// <param name="name">Name of the matrix</param>
		void SetPixelShaderMatrix(Matrix matrix, String^ name)
		{
			LPCSTR pMatrixName = (LPCSTR)Marshal::StringToHGlobalAnsi(name).ToPointer();

			D3DXMATRIX nativeMatrix(matrix.M11, matrix.M12, matrix.M13, matrix.M14,
									matrix.M21, matrix.M22, matrix.M23, matrix.M24,
									matrix.M31, matrix.M32, matrix.M33, matrix.M34,
									matrix.M41, matrix.M42, matrix.M43, matrix.M44);

			HRESULT hr = m_impl->SetPixelShaderMatrix(&nativeMatrix, pMatrixName);
			Marshal::ThrowExceptionForHR(hr);
		}

		/// <summary>
		/// Sets vector variable for vertex shader
		/// </summary>
		/// <param name="vector">Vector to set</param>
		/// <param name="name">Name of the vector</param>
		void SetVertexShaderVector(Vector4 vector, String^ name)
		{
			LPCSTR pVectorName = (LPCSTR)Marshal::StringToHGlobalAnsi(name).ToPointer();
			D3DXVECTOR4 nativeVector(vector.X, vector.Y, vector.Z, vector.W);

			HRESULT hr = m_impl->SetVertexShaderVector(&nativeVector, pVectorName);
			Marshal::ThrowExceptionForHR(hr);
		}

		/// <summary>
		/// Sets vector variable for pixel shader
		/// </summary>
		/// <param name="vector">Vector to set</param>
		/// <param name="name">Name of the vector</param>
		void SetPixelShaderVector(Vector4 vector, String^ name)
		{
			LPCSTR pVectorName = (LPCSTR)Marshal::StringToHGlobalAnsi(name).ToPointer();
			D3DXVECTOR4 nativeVector(vector.X, vector.Y, vector.Z, vector.W);

			HRESULT hr = m_impl->SetPixelShaderVector(&nativeVector, pVectorName);
			Marshal::ThrowExceptionForHR(hr);
		}

		/// <summary>
		/// Applies multiplication of World, View and Projection matrices on given matrix variable in vertex shader.
		/// </summary>
		/// <param name="matrixName">Name of the WorldViewProj matrix</param>
		void ApplyWorldViewProjMatrix(String^ matrixName)
		{
			LPCSTR pMatrixName = (LPCSTR)Marshal::StringToHGlobalAnsi(matrixName).ToPointer();
			HRESULT hr = m_impl->ApplyWorldViewProj(pMatrixName);
			Marshal::ThrowExceptionForHR(hr);
		}

		/// <summary>
		/// Adds text overlay
		/// </summary>
		/// <param name="key">Key of the ovelay object</param>
		/// <param name="text">The text to display</param>
		/// <param name="rectangle">Position in display window coordinates</param>
		/// <param name="size">Size of the text</param>
		/// <param name="color">Color of the text overlay</param>
		/// <param name="font">Font of the text</param>
		/// <param name="opacity">Opacity key: 255 - opaque, 0 - transparent</param>
		/// <remarks>Adding 2 overlays with the same key will delete the first overlay from the overlay map</remarks>
		void AddTextOverlay(short key, String^ text, System::Drawing::Rectangle rectangle, int size, System::Drawing::Color color, String^ font, System::Byte opacity)
		{
			LPCWSTR pText = (LPCWSTR)Marshal::StringToHGlobalUni(text).ToPointer();
			LPCWSTR pFont = (LPCWSTR)Marshal::StringToHGlobalUni(font).ToPointer();

			RECT rect;
			rect.top = rectangle.Top;
			rect.bottom = rectangle.Bottom;
			rect.left = rectangle.Left;
			rect.right = rectangle.Right;

			m_impl->DrawText(key, pText, rect, size, color.ToArgb(),pFont, opacity);
		}

		/// <summary>
		/// Adds line overlay
		/// </summary>
		/// <param name="key">Key of the ovelay object</param>
		/// <param name="p1">The from point of the line</param>
		/// <param name="p2">The to point of the line</param>
		/// <param name="width">Width of the line in pixels</param>
		/// <param name="color">Color of the text overlay</param>
		/// <param name="opacity">Opacity key: 255 - opaque, 0 - transparent</param>
		/// <remarks>Adding 2 overlays with the same key will delete the first overlay from the overlay map</remarks>
		void AddLineOverlay(short key, System::Drawing::Point p1, System::Drawing::Point p2, float width, System::Drawing::Color color, System::Byte opacity)
		{
			POINT pp1, pp2;
			pp1.x = p1.X;
			pp1.y = p1.Y;
			pp2.x = p2.X;
			pp2.y = p2.Y;

			m_impl->DrawLine(key, pp1, pp2, width, color.ToArgb(), opacity);
		}

		/// <summary>
		/// Adds rectangle overlay
		/// </summary>
		/// <param name="key">Key of the ovelay object</param>
		/// <param name="rectangle">Coordinates of the rectangle in display window</param>
		/// <param name="width">Width of the line in pixels</param>
		/// <param name="color">Color of the text overlay</param>
		/// <param name="opacity">Opacity key: 255 - opaque, 0 - transparent</param>
		/// <remarks>Adding 2 overlays with the same key will delete the first overlay from the overlay map</remarks>
		void AddRectangleOverlay(short key, System::Drawing::Rectangle rectangle, float width, System::Drawing::Color color, System::Byte opacity)
		{
			RECT rect;
			rect.top = rectangle.Top;
			rect.bottom = rectangle.Bottom;
			rect.left = rectangle.Left;
			rect.right = rectangle.Right;

			m_impl->DrawRectangle(key, rect, width, color.ToArgb(), opacity);
		}

		/// <summary>
		/// Adds rectangle overlay
		/// </summary>
		/// <param name="key">Key of the ovelay object</param>
		/// <param name="points">Array of points to form an polygon</param>
		/// <param name="width">Width of the line in pixels</param>
		/// <param name="color">Color of the text overlay</param>
		/// <param name="opacity">Opacity key: 255 - opaque, 0 - transparent</param>
		/// <remarks>Adding 2 overlays with the same key will delete the first overlay from the overlay map</remarks>
		void AddPolygonOverlay(short key, array<System::Drawing::Point>^ points, float width, System::Drawing::Color color, System::Byte opacity)
		{
			POINT* pointsArray = new POINT[points->Length];
			for(int i = 0 ; i < points->Length; i++)
			{
				pointsArray[i].x = points[i].X;
				pointsArray[i].y = points[i].Y;
			}

			m_impl->DrawPolygon(key, pointsArray, points->Length, width, color.ToArgb(), opacity);
		}

		/// <summary>
		/// Adds rectangle overlay
		/// </summary>
		/// <param name="key">Key of the ovelay object</param>
		/// <param name="pos">Coordinates of the rectangle in display window</param>
		/// <param name="bitmap">Bitmap to blend with the video frame</param>
		/// <param name="opacity">Opacity key: 255 - opaque, 0 - transparent</param>
		/// <remarks>Bitmap MUST be in 32 bits per pixel format. 
		/// Adding 2 overlays with the same key will delete the first overlay from the overlay map</remarks>
		void AddBitmapOverlay(short key, System::Drawing::Point pos, System::Drawing::Bitmap^ bitmap, System::Byte opacity)
		{
			POINT pp1;
			pp1.x = pos.X;
			pp1.y = pos.Y;

			System::Drawing::Rectangle rect = System::Drawing::Rectangle(0, 0, bitmap->Width, bitmap->Height);
		    System::Drawing::Imaging::BitmapData^ bmpData = bitmap->LockBits( rect, System::Drawing::Imaging::ImageLockMode::ReadOnly, bitmap->PixelFormat);

		    HRESULT hr = m_impl->DrawBitmap(key, pp1, bitmap->Width, bitmap->Height, (BYTE*)bmpData->Scan0.ToPointer(), opacity);
			bitmap->UnlockBits( bmpData );

			Marshal::ThrowExceptionForHR(hr);
		}

		/// <summary>
		/// Removes overlay from overlay map
		/// </summary>
		/// <param name="key">Key of the overlay object</param>
		void RemoveOverlay(short key)
		{
			m_impl->RemoveOverlay(key);
		}

		/// <summary>
		/// Removes all overlays
		/// </summary>
		void RemoveAllOverlays()
		{
			m_impl->RemoveAllOverlays();
		}

		/// <summary>
		/// Gets current frame either in raw video size or stretch to display window size
		/// </summary>
		/// <param name="pBuffer">Buffer for the pixel data</param>
		/// <param name="width">Width of the video frame</param>
		/// <param name="height">Height of the video frame</param>
		/// <param name="stride">Stride of the video frame (always 4 * width since it uses ARGB32)</param>
		/// <param name="bVideoFrame">True - to get video frame before display, False - to get video frame after stretch to window size and other effects applied</param>
		/// <remarks>You have to call this method once with pBuffer set to NULL (IntPtr.Zero) to get the width, height and stride. 
		/// Then, allocate appropriate buffer on the native heap and path the pointer to the buffer by calling this method second time</remarks>
		void GetCurrentFrame(System::IntPtr pBuffer,[OutAttribute] int% width, [OutAttribute] int% height, [OutAttribute] int% stride, bool bVideoFrame)
		{
			if(bVideoFrame)
			{
				int w, h, s;
				if(pBuffer == IntPtr::Zero)
				{
					HRESULT hr = m_impl->CaptureVideoFrame(NULL, &w, &h, &s);
					Marshal::ThrowExceptionForHR(hr);
					width = w;
					height = h;
					stride = s;
					return;
				}

				HRESULT hr = m_impl->CaptureVideoFrame((BYTE*)pBuffer.ToPointer(), 0, 0, 0);
				Marshal::ThrowExceptionForHR(hr);
			}
			else
			{
				int w, h, s;
				if(pBuffer == IntPtr::Zero)
				{
					HRESULT hr = m_impl->CaptureDisplayFrame(NULL, &w, &h, &s);
					Marshal::ThrowExceptionForHR(hr);
					width = w;
					height = h;
					stride = s;
					return;
				}

				HRESULT hr = m_impl->CaptureDisplayFrame((BYTE*)pBuffer.ToPointer(), 0, 0, 0);
				Marshal::ThrowExceptionForHR(hr);
			}		
		}

		/// <summary>
		/// Removes pixel shader from the rendering pipeline
		/// </summary>
		void ClearPixelShader()
		{
			HRESULT hr = m_impl->ClearPixelShader();
			Marshal::ThrowExceptionForHR(hr);
		}

		/// <summary>
		/// Removes vertex shader from the rendering pipeline
		/// </summary>
		void ClearVertexShader()
		{
			HRESULT hr = m_impl->ClearVertexShader();
			Marshal::ThrowExceptionForHR(hr);
		}

		/// <summary>
		/// Gets or sets video display mode.
		/// </summary>
		/// <remarks>By deafult set to 0 i.e. keep aspect ratio of the video frame. Set to 1 to stretch the video to entire display window size</remarks>
		property VideoDisplayMode DisplayMode
		{
		   VideoDisplayMode get()
		   {
		  	   return (VideoDisplayMode)m_impl->GetDisplayMode();
		   }
		   void set(VideoDisplayMode value)
		   {
			   m_impl->SetDisplayMode((FillMode)value);
		   }
		}

	private:
		D3D9RenderImpl* m_impl;
	};
}
}
