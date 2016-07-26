private static unsafe void YUV2RGBManaged2(byte[] YUVData, byte[] RGBData, int width, int height)
{

	fixed (byte* pRGBs = RGBData, pYUVs = YUVData)
	{
		int yIndex = 0;
		int uIndex = width * height;
		int vIndex = (width * height * 5 ) / 4;


		for (int r = 0; r < height; r++)
		{
			byte* pRGB = pRGBs + r * width * 3;

			if (r % 2 != 0)
			{
				uIndex -= (width >> 1);
				vIndex -= (width >> 1);
			}
			for (int c = 0; c < width; c += 2)
			{
				int C1 = pYUVs[yIndex++] - 16;
				int C2 = pYUVs[yIndex++] - 16;


				int D = pYUVs[vIndex++] - 128;
				int E = pYUVs[uIndex++] - 128;

				int R1 = (298 * C1 + 409 * E + 128) >> 8;
				int G1 = (298 * C1 - 100 * D - 208 * E + 128) >> 8;
				int B1 = (298 * C1 + 516 * D + 128) >> 8;

				int R2 = (298 * C2 + 409 * E + 128) >> 8;
				int G2 = (298 * C2 - 100 * D - 208 * E + 128) >> 8;
				int B2 = (298 * C2 + 516 * D + 128) >> 8;


				pRGB[0] = (byte)(R1 < 0 ? 0 : R1 > 255 ? 255 : R1);
				pRGB[1] = (byte)(G1 < 0 ? 0 : G1 > 255 ? 255 : G1);
				pRGB[2] = (byte)(B1 < 0 ? 0 : B1 > 255 ? 255 : B1);

				pRGB[3] = (byte)(R2 < 0 ? 0 : R2 > 255 ? 255 : R2);
				pRGB[4] = (byte)(G2 < 0 ? 0 : G2 > 255 ? 255 : G2);
				pRGB[5] = (byte)(B2 < 0 ? 0 : B2 > 255 ? 255 : B2);


				pRGB += 6;

			}
		}
	}
}