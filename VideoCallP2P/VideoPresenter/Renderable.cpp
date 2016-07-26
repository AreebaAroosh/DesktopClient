#pragma once 

#include "stdafx.h"
#include "IRenderable.h"
#include "D3D9RenderImpl.h"

#pragma comment(linker, "/export:GetImplementation=_GetImplementation@0")

extern "C" EXPORTLIB IRenderable* APIENTRY GetImplementation()
{
	return new D3D9RenderImpl();
}