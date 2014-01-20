using System;
using System.IO;

namespace AVR_Simulator
{
	public static class IntelHEX
	{
		public static byte[] Parse(string Path)
		{
			using (Stream FS = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read))
			using (TextReader TR = new StreamReader(FS))
			{
				string Line = TR.ReadLine();

				if (Line == null || !Line.StartsWith(":"))
					throw new InvalidDataException();

				// Maximum for 16-bit pointer
				byte[] Program = new byte[64 * 1024];

				do
				{
					if (!Line.StartsWith(":"))
						continue;

					byte ByteCount = Convert.ToByte(Line.Substring(1, 2), 16);
					ushort Address = Convert.ToUInt16(Line.Substring(3, 4), 16);
					byte RecordType = Convert.ToByte(Line.Substring(7, 2), 16);

					switch (RecordType)
					{
						case 0x00:
							// 00, data record, contains data and 16-bit address. The format described above.
							byte[] Data = new byte[ByteCount];

							for (int i = 0; i < ByteCount; i++)
								Data[i] = Convert.ToByte(Line.Substring(9 + i * 2, 2), 16);

							Buffer.BlockCopy(Data, 0, Program, Address, ByteCount);
							break;
						case 0x01:
							// End Of File record. Must occur exactly once per file in the last line of the file.
							// The byte count is 00 and the data field is empty.
							// Usually the address field is also 0000, in which case the complete line is ':00000001FF'.
							// Originally the End Of File record could contain a start address for the program being loaded, e.g. :00AB2F0125 would cause a jump to address AB2F.
							// This was convenient when programs were loaded from punched paper tape.
							break;
						case 0x02:
							// 02, Extended Segment Address Record, segment-base address (two hex digit pairs in big endian order).
							// Used when 16 bits are not enough, identical to 80x86 real mode addressing.
							// The address specified by the data field of the most recent 02 record is multiplied by 16 (shifted 4 bits left) and added to the subsequent 00 record addresses.
							// This allows addressing of up to a megabyte of address space.
							// The address field of this record has to be 0000, the byte count is 02 (the segment is 16-bit).
							// The least significant hex digit of the segment address is always 0.
							break;
						case 0x03:
							// 03, Start Segment Address Record. For 80x86 processors, it specifies the initial content of the CS:IP registers.
							// The address field is 0000, the byte count is 04, the first two bytes are the CS value, the latter two are the IP value.
							break;
						case 0x04:
							// 04, Extended Linear Address Record, allowing for fully 32 bit addressing (up to 4GiB).
							// The address field is 0000, the byte count is 02.
							// The two data bytes (two hex digit pairs in big endian order) represent the upper 16 bits of the 32 bit address for all subsequent 00 type records until the next 04 type record comes.
							// If there is not a 04 type record, the upper 16 bits default to 0000.
							// To get the absolute address for subsequent 00 type records, the address specified by the data field of the most recent 04 record is added to the 00 record addresses.
							break;
						case 0x05:
							// 05, Start Linear Address Record. The address field is 0000, the byte count is 04.
							// The 4 data bytes represent the 32-bit value loaded into the EIP register of the 80386 and higher CPU.
							break;
					}
				}
				while ((Line = TR.ReadLine()) != null);

				return Program;
			}
		}
	}
}