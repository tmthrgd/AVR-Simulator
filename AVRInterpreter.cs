using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace AVR_Simulator
{
	public abstract class AVRInterpreter
	{
		protected AVRInterpreter()
		{
			this.Instructions = new InstructionFunc[]
			{
				new InstructionFunc { Mask = 0xFC00, OpCode = 0x1C00, Func = this.ADC },
				new InstructionFunc { Mask = 0xFC00, OpCode = 0x0C00, Func = this.ADD },
				new InstructionFunc { Mask = 0xFF00, OpCode = 0x9600, Func = this.ADIW },
				new InstructionFunc { Mask = 0xFC00, OpCode = 0x2000, Func = this.AND },
				new InstructionFunc { Mask = 0xF000, OpCode = 0x7000, Func = this.ANDI },
				new InstructionFunc { Mask = 0xFE0F, OpCode = 0x9405, Func = this.ASR },
				new InstructionFunc { Mask = 0xFF8F, OpCode = 0x9488, Func = this.BCLR },
				new InstructionFunc { Mask = 0xFE08, OpCode = 0xF800, Func = this.BLD },
				new InstructionFunc { Mask = 0xFC00, OpCode = 0xF400, Func = this.BRBC },
				new InstructionFunc { Mask = 0xFC00, OpCode = 0xF000, Func = this.BRBS },
				// BRCC (See BRBC)
				// BRCS (See BRBS)
				new InstructionFunc { Mask = 0xFFFF, OpCode = 0x9598, Func = this.BREAK },
				// BREQ (See BRBS)
				// BRGE (See BRBC)
				// BRHC (See BRBC)
				// BRHS (See BRBS)
				// BRID (See BRBC)
				// BRIE (See BRBS)
				// BRLO (See BRBS)
				// BRLT (See BRBS)
				// BRMI (See BRBS)
				// BRNE (See BRBC)
				// BRPL (See BRBC)
				// BRSH (See BRBC)
				// BRTC (See BRBC)
				// BRTS (See BRBS)
				// BRVC (See BRBC)
				// BRVS (See BRBS)
				new InstructionFunc { Mask = 0xFF8F, OpCode = 0x9408, Func = this.BSET },
				new InstructionFunc { Mask = 0xFE08, OpCode = 0xFA00, Func = this.BST },
				new InstructionFunc { Mask = 0xFE0E, OpCode = 0x940E, Func = this.CALL, Double = true },
				new InstructionFunc { Mask = 0xFF00, OpCode = 0x9800, Func = this.CBI },
				// CBR (See ANDI)
				// CLC (See BCLR)
				// CLH (See BCLR)
				// CLI (See BCLR)
				// CLN (See BCLR)
				// CLR (See EOR)
				// CLS (See BCLR)
				// CLT (See BCLR)
				// CLV (See BCLR)
				// CLZ (See BCLR)
				new InstructionFunc { Mask = 0xFE0F, OpCode = 0x9400, Func = this.COM },
				new InstructionFunc { Mask = 0xFC00, OpCode = 0x1400, Func = this.CP },
				new InstructionFunc { Mask = 0xFC00, OpCode = 0x0400, Func = this.CPC },
				new InstructionFunc { Mask = 0xF000, OpCode = 0x3000, Func = this.CPI },
				new InstructionFunc { Mask = 0xFC00, OpCode = 0x1000, Func = this.CPSE },
				new InstructionFunc { Mask = 0xFE0F, OpCode = 0x940A, Func = this.DEC },
				new InstructionFunc { Mask = 0xFF0F, OpCode = 0x940B, Func = this.DES },
				new InstructionFunc { Mask = 0xFFFF, OpCode = 0x9519, Func = this.EICALL },
				new InstructionFunc { Mask = 0xFFFF, OpCode = 0x9419, Func = this.EIJMP },
				new InstructionFunc { Mask = 0xFFFF, OpCode = 0x95D8, Func = this.ELPM_1 },
				new InstructionFunc { Mask = 0xFE0F, OpCode = 0x9006, Func = this.ELPM_2 },
				new InstructionFunc { Mask = 0xFE0F, OpCode = 0x9007, Func = this.ELPM_3 },
				new InstructionFunc { Mask = 0xFC00, OpCode = 0x2400, Func = this.EOR },
				new InstructionFunc { Mask = 0xFF88, OpCode = 0x0308, Func = this.FMUL },
				new InstructionFunc { Mask = 0xFF88, OpCode = 0x0380, Func = this.FMULS },
				new InstructionFunc { Mask = 0xFF88, OpCode = 0x0388, Func = this.FMULSU },
				new InstructionFunc { Mask = 0xFFFF, OpCode = 0x9509, Func = this.ICALL },
				new InstructionFunc { Mask = 0xFFFF, OpCode = 0x9409, Func = this.IJMP },
				new InstructionFunc { Mask = 0xF800, OpCode = 0xB000, Func = this.IN },
				new InstructionFunc { Mask = 0xFE0F, OpCode = 0x9403, Func = this.INC },
				new InstructionFunc { Mask = 0xFE0E, OpCode = 0x940C, Func = this.JMP, Double = true },
				new InstructionFunc { Mask = 0xFE0F, OpCode = 0x9206, Func = this.LAC },
				new InstructionFunc { Mask = 0xFE0F, OpCode = 0x9205, Func = this.LAS },
				new InstructionFunc { Mask = 0xFE0F, OpCode = 0x9207, Func = this.LAT },
				new InstructionFunc { Mask = 0xFE0F, OpCode = 0x900C, Func = this.LD_X1 },
				new InstructionFunc { Mask = 0xFE0F, OpCode = 0x900D, Func = this.LD_X2 },
				new InstructionFunc { Mask = 0xFE0F, OpCode = 0x900E, Func = this.LD_X3 },
				// LD_Y1 (See LDD_Y)
				new InstructionFunc { Mask = 0xFE0F, OpCode = 0x9009, Func = this.LD_Y2 },
				new InstructionFunc { Mask = 0xFE0F, OpCode = 0x900A, Func = this.LD_Y3 },
				new InstructionFunc { Mask = 0xD208, OpCode = 0x8008, Func = this.LDD_Y },
				// LD_Z1 (See LDD_Z)
				new InstructionFunc { Mask = 0xFE0F, OpCode = 0x9001, Func = this.LD_Z2 },
				new InstructionFunc { Mask = 0xFE0F, OpCode = 0x9002, Func = this.LD_Z3 },
				new InstructionFunc { Mask = 0xD208, OpCode = 0x8000, Func = this.LDD_Z },
				new InstructionFunc { Mask = 0xF000, OpCode = 0xE000, Func = this.LDI },
				new InstructionFunc { Mask = 0xFE0F, OpCode = 0x9000, Func = this.LDS, Double = true },
				new InstructionFunc { Mask = 0xF800, OpCode = 0xA000, Func = this.LDS_16bit },
				new InstructionFunc { Mask = 0xFFFF, OpCode = 0x95C8, Func = this.LPM_1 },
				new InstructionFunc { Mask = 0xFE0F, OpCode = 0x9004, Func = this.LPM_2 },
				new InstructionFunc { Mask = 0xFE0F, OpCode = 0x9005, Func = this.LPM_3 },
				// LSL (See ADD)
				new InstructionFunc { Mask = 0xFE0F, OpCode = 0x9406, Func = this.LSR },
				new InstructionFunc { Mask = 0xFC00, OpCode = 0x2C00, Func = this.MOV },
				new InstructionFunc { Mask = 0xFF00, OpCode = 0x0100, Func = this.MOVW },
				new InstructionFunc { Mask = 0xFC00, OpCode = 0x9C00, Func = this.MUL },
				new InstructionFunc { Mask = 0xFF00, OpCode = 0x0200, Func = this.MULS },
				new InstructionFunc { Mask = 0xFF88, OpCode = 0x0300, Func = this.MULSU },
				new InstructionFunc { Mask = 0xFE0F, OpCode = 0x9401, Func = this.NEG },
				new InstructionFunc { Mask = 0xFFFF, OpCode = 0x0000, Func = this.NOP },
				new InstructionFunc { Mask = 0xFC00, OpCode = 0x2800, Func = this.OR },
				new InstructionFunc { Mask = 0xF000, OpCode = 0x6000, Func = this.ORI },
				new InstructionFunc { Mask = 0xF800, OpCode = 0xB800, Func = this.OUT },
				new InstructionFunc { Mask = 0xFE0F, OpCode = 0x900F, Func = this.POP },
				new InstructionFunc { Mask = 0xFE0F, OpCode = 0x920F, Func = this.PUSH },
				new InstructionFunc { Mask = 0xF000, OpCode = 0xD000, Func = this.RCALL },
				new InstructionFunc { Mask = 0xFFFF, OpCode = 0x9508, Func = this.RET },
				new InstructionFunc { Mask = 0xFFFF, OpCode = 0x9518, Func = this.RETI },
				new InstructionFunc { Mask = 0xF000, OpCode = 0xC000, Func = this.RJMP },
				// ROL (See ADC)
				new InstructionFunc { Mask = 0xFE0F, OpCode = 0x9407, Func = this.ROR },
				new InstructionFunc { Mask = 0xFC00, OpCode = 0x0800, Func = this.SBC },
				new InstructionFunc { Mask = 0xF000, OpCode = 0x4000, Func = this.SBCI },
				new InstructionFunc { Mask = 0xFF00, OpCode = 0x9A00, Func = this.SBI },
				new InstructionFunc { Mask = 0xFF00, OpCode = 0x9900, Func = this.SBIC },
				new InstructionFunc { Mask = 0xFF00, OpCode = 0x9B00, Func = this.SBIS },
				new InstructionFunc { Mask = 0xFF00, OpCode = 0x9700, Func = this.SBIW },
				// SBR (See ORI)
				new InstructionFunc { Mask = 0xFE08, OpCode = 0xFC00, Func = this.SBRC },
				new InstructionFunc { Mask = 0xFE08, OpCode = 0xFE00, Func = this.SBRS },
				// SEC (See BSET)
				// SEH (See BSET)
				// SEI (See BSET)
				// SEN (See BSET)
				// SER (See LDI)
				// SES (See BSET)
				// SET (See BSET)
				// SEV (See BSET)
				// SEZ (See BSET)
				new InstructionFunc { Mask = 0xFFFF, OpCode = 0x9588, Func = this.SLEEP },
				new InstructionFunc { Mask = 0xFFFF, OpCode = 0x95E8, Func = this.SPM },
				new InstructionFunc { Mask = 0xFFFF, OpCode = 0x95E8, Func = this.SPM2_1_3 },
				new InstructionFunc { Mask = 0xFFFF, OpCode = 0x95F8, Func = this.SPM2_4_6 },
				new InstructionFunc { Mask = 0xFE0F, OpCode = 0x920C, Func = this.ST_X1 },
				new InstructionFunc { Mask = 0xFE0F, OpCode = 0x920D, Func = this.ST_X2 },
				new InstructionFunc { Mask = 0xFE0F, OpCode = 0x920E, Func = this.ST_X3 },
				// ST_Y1 (See STD_Y)
				new InstructionFunc { Mask = 0xFE0F, OpCode = 0x9209, Func = this.ST_Y2 },
				new InstructionFunc { Mask = 0xFE0F, OpCode = 0x920A, Func = this.ST_Y3 },
				new InstructionFunc { Mask = 0xD208, OpCode = 0x8208, Func = this.STD_Y },
				// ST_Z1 (See STD_Z)
				new InstructionFunc { Mask = 0xFE0F, OpCode = 0x9201, Func = this.ST_Z2 },
				new InstructionFunc { Mask = 0xFE0F, OpCode = 0x9202, Func = this.ST_Z3 },
				new InstructionFunc { Mask = 0xD208, OpCode = 0x8200, Func = this.STD_Z },
				new InstructionFunc { Mask = 0xFE0F, OpCode = 0x9200, Func = this.STS, Double = true },
				new InstructionFunc { Mask = 0xF800, OpCode = 0xA800, Func = this.STS_16bit },
				new InstructionFunc { Mask = 0xFC00, OpCode = 0x1800, Func = this.SUB },
				new InstructionFunc { Mask = 0xF000, OpCode = 0x5000, Func = this.SUBI },
				new InstructionFunc { Mask = 0xFE0F, OpCode = 0x9402, Func = this.SWAP },
				// TST (See AND)
				new InstructionFunc { Mask = 0xFFFF, OpCode = 0x95A8, Func = this.WDR },
				new InstructionFunc { Mask = 0xFE0F, OpCode = 0x9204, Func = this.XCH },
			};

			this.UnkownInstruction = new InstructionFunc
			{
				Mask = 0x0000,
				OpCode = 0xFFFF,
				Func = () =>
				{
					Console.WriteLine("Unkown instruction 0x{0:X4}", this.Instruction);
					this.PC++;
				}
			};
			
			this.InstructionMap = Enumerable.Range(0, 0xFFFF)
				.Select(i => this.Instructions.FirstOrDefault(func => (i & func.Mask) == func.OpCode) ?? this.UnkownInstruction)
				.ToArray();
			
			this.PC = 0;
		}

		protected InstructionFunc[] Instructions { get; set; }
		protected InstructionFunc UnkownInstruction { get; set; }

		protected InstructionFunc[] InstructionMap { get; set; }

		public event NotifyCollectionChangedEventHandler RAMChanged
		{
			add
			{
				this.RAM.CollectionChanged += value;
			}
			remove
			{
				this.RAM.CollectionChanged -= value;
			}
		}

		public ushort[] Flash { get; protected set; }
		public byte[] EEPROM { get; protected set; }
		public ObservableCollection<byte> RAM { get; protected set; }
		public MappedArray<byte> R { get; protected set; }
		public MappedArray<byte> IO { get; protected set; }
		public MappedArray<byte> SRAM { get; protected set; }

		public int PC { get; set; }
		public ushort Instruction { get; set; }
		public ushort Instruction2 { get; set; }
		
		// Registers
		public virtual ushort X
		{
			get
			{
				return (ushort)((this.R[27] << 8) | this.R[26]);
			}
			set
			{
				this.R[27] = (byte)(value >> 8);
				this.R[26] = (byte)(value & 0xFF);
			}
		}
		public virtual ushort Y
		{
			get
			{
				return (ushort)((this.R[29] << 8) | this.R[28]);
			}
			set
			{
				this.R[28] = (byte)(value >> 8);
				this.R[29] = (byte)(value & 0xFF);
			}
		}
		public virtual ushort Z
		{
			get
			{
				return (ushort)((this.R[31] << 8) | this.R[30]);
			}
			set
			{
				this.R[31] = (byte)(value >> 8);
				this.R[30] = (byte)(value & 0xFF);
			}
		}
		
		// I/O
		public virtual byte RAMPD
		{
			get
			{
				return this.IO[0x38];
			}
			set
			{
				this.IO[0x38] = value;
			}
		}
		public virtual byte RAMPX
		{
			get
			{
				return this.IO[0x39];
			}
			set
			{
				this.IO[0x39] = value;
			}
		}
		public virtual byte RAMPY
		{
			get
			{
				return this.IO[0x3A];
			}
			set
			{
				this.IO[0x3A] = value;
			}
		}
		public virtual byte RAMPZ
		{
			get
			{
				return this.IO[0x3B];
			}
			set
			{
				this.IO[0x3B] = value;
			}
		}
		public virtual byte EIND
		{
			get
			{
				return this.IO[0x3C];
			}
			set
			{
				this.IO[0x3C] = value;
			}
		}
		public virtual ushort SP
		{
			get
			{
				if (this.RAM.Count > 256)
					return (ushort)((this.IO[0x3E] << 8) | this.IO[0x3D]);
				else
					return this.IO[0x3D];
			}
			set
			{
				if (this.RAM.Count > 256)
					this.IO[0x3E] = (byte)(value >> 8);

				this.IO[0x3D] = (byte)(value & 0xFF);
			}
		}
		public virtual byte SREG
		{
			get
			{
				return this.IO[0x3F];
			}
			set
			{
				this.IO[0x3F] = value;
			}
		}

		// Conglomorate
		protected virtual int RAMPXX
		{
			get
			{
				return (this.RAMPX << 16) | this.X;
			}
			set
			{
				this.RAMPX = (byte)((value & 0xFF0000) >> 16);
				this.X = (ushort)(value & 0x00FFFF);
			}
		}
		protected virtual int RAMPYY
		{
			get
			{
				return (this.RAMPY << 16) | this.Y;
			}
			set
			{
				this.RAMPY = (byte)((value & 0xFF0000) >> 16);
				this.Y = (ushort)(value & 0x00FFFF);
			}
		}
		protected virtual int RAMPZZ
		{
			get
			{
				return (this.RAMPZ << 16) | this.Z;
			}
			set
			{
				this.RAMPZ = (byte)((value & 0xFF0000) >> 16);
				this.Z = (ushort)(value & 0x00FFFF);
			}
		}
		protected virtual int EINDZ
		{
			get
			{
				return (this.EIND << 16) | this.Z;
			}
			set
			{
				this.EIND = (byte)((value & 0xFF0000) >> 16);
				this.Z = (ushort)(value & 0x00FFFF);
			}
		}

		public virtual void Reset()
		{
			Array.Clear(this.Flash, 0, this.Flash.Length);
			Array.Clear(this.EEPROM, 0, this.EEPROM.Length);
			this.RAM.Clear();

			this.SP = (ushort)this.SRAM.End;
			this.PC = 0;
		}

		#region Load
		public virtual void Load(string FlashPath)
		{
			this.Load(File.ReadAllBytes(FlashPath));
		}

		public virtual void Load(byte[] Flash)
		{
			Buffer.BlockCopy(Flash, 0, this.Flash, 0, Math.Min(Buffer.ByteLength(this.Flash), Flash.Length));
		}

		/*public virtual void Load(string FlashPath, string EEPROMPath)
		{
			this.Load(File.ReadAllBytes(FlashPath), File.ReadAllBytes(EEPROMPath));
		}

		public virtual void Load(byte[] Flash, byte[] EEPROM)
		{
			Buffer.BlockCopy(Flash, 0, this.Flash, 0, Math.Min(Buffer.ByteLength(this.Flash), Flash.Length));
			Buffer.BlockCopy(EEPROM, 0, this.EEPROM, 0, Math.Min(this.EEPROM.Length, EEPROM.Length));
		}*/
		#endregion

		public virtual void Execute()
		{
			this.Instruction = this.Flash[this.PC];
			this.Instruction2 = this.Flash[this.PC + 1];

			this.InstructionMap[this.Instruction].Func();
		}

		protected class InstructionFunc
		{
			public ushort Mask { get; set; }
			public ushort OpCode { get; set; }
			public Action Func { get; set; }
			public bool Double { get; set; }

			public override string ToString()
			{
				return this.Func.Method.Name;
			}
		}

		#region GPIO
		public class GPIO
		{
			public IList<byte> IO;
			public int DDRx;
			public int PORTx;
			public int PINx;
			public byte nMask;

			public event EventHandler<GPIOValueChangedEventArgs> ValueChanged;

			public byte Value
			{
				get
				{
					return this.IO[this.PINx];
				}
				set
				{
					this.IO[this.PINx] = value;
				}
			}

			public byte PullUp
			{
				get
				{
					return 0;
				}
				set
				{

				}
			}

			public virtual void InvokeValueChanged(GPIOValueChangedEventArgs e)
			{
				if (this.ValueChanged != null && e.OldValue != e.NewValue)
					this.ValueChanged(this, e);
			}

			public override string ToString()
			{
				return string.Format("0x{0:X2}", this.Value);
			}
		}

		public class GPIOPin
		{
			public IList<byte> IO;
			public int DDRx;
			public int PORTx;
			public int PINx;
			public byte nMask;

			public event EventHandler<GPIOPinValueChangedEventArgs> ValueChanged;

			public GPIOPinDirection Direction
			{
				get
				{
					return ((this.IO[this.DDRx] & this.nMask) == this.nMask)
						? GPIOPinDirection.OUTPUT
						: GPIOPinDirection.INPUT;
				}
				set
				{
					if (value == GPIOPinDirection.OUTPUT)
						this.IO[this.DDRx] |= this.nMask;
					else
						this.IO[this.DDRx] &= (byte)~this.nMask;
				}
			}

			public bool Value
			{
				get
				{
					return (this.IO[this.PINx] & this.nMask) == this.nMask;
				}
				set
				{
					int idx = (this.Direction == GPIOPinDirection.INPUT)
						? this.PINx
						: this.PORTx;

					if (value)
						this.IO[idx] |= this.nMask;
					else
						this.IO[idx] &= (byte)~this.nMask;
				}
			}

			public bool PullUp
			{
				get
				{
					return false;
				}
				set
				{

				}
			}

			public void InvokeValueChanged(GPIOPinValueChangedEventArgs e)
			{
				if (this.ValueChanged != null && e.OldValue != e.NewValue)
					this.ValueChanged(this, e);
			}

			public override string ToString()
			{
				return this.Value.ToString();
			}
		}

		public class GPIOValueChangedEventArgs : EventArgs
		{
			public GPIOValueChangedEventArgs(byte OldValue, byte NewValue)
			{
				this.OldValue = OldValue;
				this.NewValue = NewValue;
			}

			public byte OldValue { get; private set; }
			public byte NewValue { get; private set; }
		}

		public class GPIOPinValueChangedEventArgs : EventArgs
		{
			public GPIOPinValueChangedEventArgs(byte OldValue, byte NewValue, byte nMask)
			{
				this.OldValue = (OldValue & nMask) == nMask;
				this.NewValue = (NewValue & nMask) == nMask;
			}

			public bool OldValue { get; private set; }
			public bool NewValue { get; private set; }
		}

		public enum GPIOPinDirection
		{
			INPUT,
			OUTPUT
		}
		#endregion

		#region Instructions
		/*
		 * Duplicates:
		 * 
		 * 0x95E8:
		 *	Void SPM(), 0xFFFF, 0x95E8
		 *	Void SPM2_1_3(), 0xFFFF, 0x95E8
		 */

		protected virtual void ADC()
		{
			// ADC 0b0001 11rd dddd rrrr
			this.PC++;
		}

		protected virtual void ADD()
		{
			// ADD 0b0000 11rd dddd rrrr
			this.PC++;
		}

		protected virtual void ADIW()
		{
			// ADIW 0b1001 0110 KKdd KKKK
			this.PC++;
		}

		protected virtual void AND()
		{
			// AND 0b0010 00rd dddd rrrr
			this.PC++;
		}

		protected virtual void ANDI()
		{
			// ANDI 0b0111 KKKK dddd KKKK
			this.PC++;
		}

		protected virtual void ASR()
		{
			// ASR 0b1001 010d dddd 0101
			this.PC++;
		}

		protected virtual void BCLR()
		{
			// BCLR 0b1001 0100 1sss 1000
			this.SREG &= (byte)~(1 << ((this.Instruction & 0x0070) >> 4));
			this.PC++;
		}

		protected virtual void BLD()
		{
			// BLD 0b1111 100d dddd 0bbb
			this.PC++;
		}

		protected virtual void BRBC()
		{
			// BRBC 0b1111 01kk kkkk ksss

			if ((this.SREG & (1 << (this.Instruction & 0x0007))) != 0)
				this.PC++;
			else
				this.PC += (((this.Instruction & 0x03F8) << 22) >> 25) + 1;
		}

		protected virtual void BRBS()
		{
			// BRBS 0b1111 00kk kkkk ksss

			if ((this.SREG & (1 << (this.Instruction & 0x0007))) != 0)
				this.PC += (((this.Instruction & 0x03F8) << 22) >> 25) + 1;
			else
				this.PC++;
		}

		// BRCC (See BRBC)

		// BRCS (See BRBS)

		protected virtual void BREAK()
		{
			// BREAK 0b1001 0101 1001 1000
			System.Diagnostics.Debugger.Break();
			this.PC++;
		}

		// BREQ (See BRBS)

		// BRGE (See BRBC)

		// BRHC (See BRBC)

		// BRHS (See BRBS)

		// BRID (See BRBC)

		// BRIE (See BRBS)

		// BRLO (See BRBS)

		// BRLT (See BRBS)

		// BRMI (See BRBS)

		// BRNE (See BRBC)

		// BRPL (See BRBC)

		// BRSH (See BRBC)

		// BRTC (See BRBC)

		// BRTS (See BRBS)

		// BRVC (See BRBC)

		// BRVS (See BRBS)

		protected virtual void BSET()
		{
			// BSET 0b1001 0100 0sss 1000
			this.SREG |= (byte)(1 << ((this.Instruction & 0x0070) >> 4));
			this.PC++;
		}

		protected virtual void BST()
		{
			// BST 0b1111 101d dddd 0bbb
			this.PC++;
		}

		protected virtual void CALL()
		{
			// CALL 0b1001 010k kkkk 111k kkkk kkkk kkkk kkkk
			this.RAM[this.SP--] = (byte)((this.PC + 2) & 0xFF);
			this.RAM[this.SP--] = (byte)(((this.PC + 2) >> 8) & 0xFF);

			if (this.Flash.Length > 0x20000)
				this.RAM[this.SP--] = (byte)(((this.PC + 2) >> 16) & 0x3F);
			
			this.PC = ((this.Instruction & 0x01F0) << 17) | ((this.Instruction & 0x0001) << 16) | this.Instruction2;
		}

		protected virtual void CBI()
		{
			// CBI 0b1001 1000 AAAA Abbb
			this.IO[(this.Instruction & 0x00F8) >> 3] &= (byte)~(1 << (this.Instruction & 0x0007));
			this.PC++;
		}

		// CBR (See ANDI)

		// CLC (See BCLR)

		// CLH (See BCLR)

		// CLI (See BCLR)

		// CLN (See BCLR)

		// CLR (See EOR)

		// CLS (See BCLR)

		// CLT (See BCLR)

		// CLV (See BCLR)

		// CLZ (See BCLR)

		protected virtual void COM()
		{
			// COM 0b1001 010d dddd 0000
			this.PC++;
		}

		protected virtual void CP()
		{
			// CP 0b0001 01rd dddd rrrr
			this.PC++;
		}

		protected virtual void CPC()
		{
			// CPC 0b0000 01rd dddd rrrr
			this.PC++;
		}

		protected virtual void CPI()
		{
			// CPI 0b0011 KKKK dddd KKKK
			this.PC++;
		}

		protected virtual void CPSE()
		{
			// CPSE 0b0001 00rd dddd rrrr
			
		}

		protected virtual void DEC()
		{
			// DEC 0b1001 010d dddd 1010
			this.PC++;
		}

		protected virtual void DES()
		{
			// DES 0b1001 0100 KKKK 1011
			this.PC++;
		}

		protected virtual void EICALL()
		{
			// EICALL 0b1001 0101 0001 1001
			this.RAM[this.SP--] = (byte)((this.PC + 1) & 0xFF);
			this.RAM[this.SP--] = (byte)(((this.PC + 1) >> 8) & 0xFF);
			
			if (this.Flash.Length > 0x20000)
				this.RAM[this.SP--] = (byte)(((this.PC + 1) >> 16) & 0x3F);
			
			this.PC = this.EINDZ;
		}

		protected virtual void EIJMP()
		{
			// EIJMP 0b1001 0100 0001 1001
			this.PC = this.EINDZ;
		}

		protected virtual void ELPM_1()
		{
			// ELPM 0b1001 0101 1101 1000
			this.PC++;
		}

		protected virtual void ELPM_2()
		{
			// ELPM 0b1001 000d dddd 0110
			this.PC++;
		}

		protected virtual void ELPM_3()
		{
			// ELPM 0b1001 000d dddd 0111
			this.PC++;
		}

		protected virtual void EOR()
		{
			// EOR 0b0010 01rd dddd rrrr
			this.PC++;
		}

		protected virtual void FMUL()
		{
			// FMUL 0b0000 0011 0ddd 1rrr
			this.PC++;
		}

		protected virtual void FMULS()
		{
			// FMULS 0b0000 0011 1ddd 0rrr
			this.PC++;
		}

		protected virtual void FMULSU()
		{
			// FMULSU 0b0000 0011 1ddd 1rrr
			this.PC++;
		}

		protected virtual void ICALL()
		{
			// ICALL 0b1001 0101 0000 1001
			this.RAM[this.SP--] = (byte)((this.PC + 1) & 0xFF);
			this.RAM[this.SP--] = (byte)(((this.PC + 1) >> 8) & 0xFF);

			if (this.Flash.Length > 0x20000)
				this.RAM[this.SP--] = (byte)(((this.PC + 1) >> 16) & 0x3F);

			this.PC = this.Z;
		}

		protected virtual void IJMP()
		{
			// IJMP 0b1001 0100 0000 1001
			this.PC = this.Z;
		}

		protected virtual void IN()
		{
			// IN 0b1011 0AAd dddd AAAA
			this.R[(this.Instruction & 0x01F0) >> 4] = this.IO[((this.Instruction & 0x0600) >> 5) | (this.Instruction & 0x000F)];
			this.PC++;
		}

		protected virtual void INC()
		{
			// INC 0b1001 010d dddd 0011
			this.PC++;
		}

		protected virtual void JMP()
		{
			// JMP 0b1001 010k kkkk 110k kkkk kkkk kkkk kkkk
			this.PC = ((this.Instruction & 0x1F0) << 17) | ((this.Instruction & 0x1) << 16) | this.Instruction2;
		}

		protected virtual void LAC()
		{
			// LAC 0b1001 001r rrrr 0110
			byte old = this.RAM[(this.RAMPZ << 16) | this.Z];
			this.RAM[(this.RAMPZ << 16) | this.Z] &= (byte)~this.R[(this.Instruction & 0x01F0) >> 4];
			this.R[(this.Instruction & 0x01F0) >> 4] = old;
			this.PC++;
		}

		protected virtual void LAS()
		{
			// LAS 0b1001 001r rrrr 0101
			byte old = this.RAM[(this.RAMPZ << 16) | this.Z];
			this.RAM[(this.RAMPZ << 16) | this.Z] |= this.R[(this.Instruction & 0x01F0) >> 4];
			this.R[(this.Instruction & 0x01F0) >> 4] = old;
			this.PC++;
		}

		protected virtual void LAT()
		{
			// LAT 0b1001 001r rrrr 0111
			byte old = this.RAM[(this.RAMPZ << 16) | this.Z];
			this.RAM[(this.RAMPZ << 16) | this.Z] ^= this.R[(this.Instruction & 0x01F0) >> 4];
			this.R[(this.Instruction & 0x01F0) >> 4] = old;
			this.PC++;
		}

		protected virtual void LD_X1()
		{
			// LD 0b1001 000d dddd 1100
			this.PC++;
		}

		protected virtual void LD_X2()
		{
			// LD 0b1001 000d dddd 1101
			this.PC++;
		}

		protected virtual void LD_X3()
		{
			// LD 0b1001 000d dddd 1110
			this.PC++;
		}

		// LD_Y1 (See LDD_Y)

		protected virtual void LD_Y2()
		{
			// LD 0b1001 000d dddd 1001
			this.PC++;
		}

		protected virtual void LD_Y3()
		{
			// LD 0b1001 000d dddd 1010
			this.PC++;
		}

		protected virtual void LDD_Y()
		{
			// LDD 0b10q0 qq0d dddd 1qqq
			this.PC++;
		}

		// LD_Z1 (See LDD_Z)

		protected virtual void LD_Z2()
		{
			// LD 0b1001 000d dddd 0001
			this.PC++;
		}

		protected virtual void LD_Z3()
		{
			// LD 0b1001 000d dddd 0010
			this.PC++;
		}

		protected virtual void LDD_Z()
		{
			// LDD 0b10q0 qq0d dddd 0qqq
			this.PC++;
		}

		protected virtual void LDI()
		{
			// LDI 0b1110 KKKK dddd KKKK
			this.R[((this.Instruction & 0x00F0) >> 4) + 16] = (byte)(((this.Instruction & 0x0F00) >> 4) | (this.Instruction & 0x000F));
			this.PC++;
		}

		protected virtual void LDS()
		{
			// LDS 0b1001 000d dddd 0000 kkkk kkkk kkkk kkkk
			this.R[(this.Instruction & 0x01F0) >> 4] = this.RAM[this.Instruction2 + this.RAMPD];
			this.PC += 2;
		}

		protected virtual void LDS_16bit()
		{
			// LDS 0b1010 0kkk dddd kkkk
			this.R[((this.Instruction & 0x00F0) >> 4) + 16]
				= this.RAM[((~this.Instruction2 & 0x0100) >> 2) | ((this.Instruction2 & 0x0100) >> 3) | ((this.Instruction2 & 0x0600) >> 5) | (this.Instruction2 & 0x000F)];
			this.PC++;
		}

		protected virtual void LPM_1()
		{
			// LPM 0b1001 0101 1100 1000
			this.PC++;
		}

		protected virtual void LPM_2()
		{
			// LPM 0b1001 000d dddd 0100
			this.PC++;
		}

		protected virtual void LPM_3()
		{
			// LPM 0b1001 000d dddd 0101
			this.PC++;
		}

		// LSL (See ADD)

		protected virtual void LSR()
		{
			// LSR 0b1001 010d dddd 0110
			this.PC++;
		}

		protected virtual void MOV()
		{
			// MOV 0b0010 11rd dddd rrrr
			this.R[(this.Instruction & 0x01F0) >> 4] = this.R[((this.Instruction & 0x0200) >> 5) | (this.Instruction & 0x000F)];
			this.PC++;
		}

		protected virtual void MOVW()
		{
			// MOVW 0b0000 0001 dddd rrrr
			this.R[((this.Instruction & 0x00F0) >> 3) | 0x0001] = this.R[((this.Instruction & 0x000F) << 1) | 0x0001];
			this.R[(this.Instruction & 0x00F0) >> 3] = this.R[(this.Instruction & 0x000F) << 1];
			this.PC++;
		}

		protected virtual void MUL()
		{
			// MUL 0b1001 11rd dddd rrrr
			this.PC++;
		}

		protected virtual void MULS()
		{
			// MULS 0b0000 0010 dddd rrrr
			this.PC++;
		}

		protected virtual void MULSU()
		{
			// MULSU 0b0000 0011 0ddd 0rrr
			this.PC++;
		}

		protected virtual void NEG()
		{
			// NEG 0b1001 010d dddd 0001
			this.PC++;
		}

		protected virtual void NOP()
		{
			// NOP 0b0000 0000 0000 0000
			this.PC++;
		}

		protected virtual void OR()
		{
			// OR 0b0010 10rd dddd rrrr
			this.PC++;
		}

		protected virtual void ORI()
		{
			// ORI 0b0110 KKKK dddd KKKK
			this.PC++;
		}

		protected virtual void OUT()
		{
			// OUT 0b1011 1AAr rrrr AAAA
			this.IO[((this.Instruction & 0x0600) >> 5) | (this.Instruction & 0x000F)] = this.R[(this.Instruction & 0x01F0) >> 4];
			this.PC++;
		}

		protected virtual void POP()
		{
			// POP 0b1001 000d dddd 1111
			this.R[(this.Instruction & 0x01F0) >> 4] = this.RAM[++this.SP];
			this.PC++;
		}

		protected virtual void PUSH()
		{
			// PUSH 0b1001 001d dddd 1111
			this.RAM[this.SP--] = this.R[(this.Instruction & 0x01F0) >> 4];
			this.PC++;
		}

		protected virtual void RCALL()
		{
			// RCALL 0b1101 kkkk kkkk kkkk
			this.RAM[this.SP--] = (byte)((this.PC + 1) & 0xFF);
			this.RAM[this.SP--] = (byte)(((this.PC + 1) >> 8) & 0xFF);

			if (this.Flash.Length > 0x20000)
				this.RAM[this.SP--] = (byte)(((this.PC + 1) >> 16) & 0x3F);

			this.PC += ((this.Instruction << 20) >> 20) + 1;
		}

		protected virtual void RET()
		{
			// RET 0b1001 0101 0000 1000
			this.PC = this.RAM[++this.SP] | this.RAM[++this.SP] << 8;

			if (this.Flash.Length > 0x20000)
				this.PC |= this.RAM[++this.SP] << 16;
		}

		protected virtual void RETI()
		{
			// RETI 0b1001 0101 0001 1000
			this.PC = this.RAM[++this.SP] | this.RAM[++this.SP] << 8;

			if (this.Flash.Length > 0x20000)
				this.PC |= this.RAM[++this.SP] << 16;

			this.SREG |= 0x80;
		}

		protected virtual void RJMP()
		{
			// RJMP 0b1100 kkkk kkkk kkkk
			this.PC += ((this.Instruction << 20) >> 20) + 1;
		}

		// ROL (See ADC)

		protected virtual void ROR()
		{
			// ROR 0b1001 010d dddd 0111
			this.PC++;
		}

		protected virtual void SBC()
		{
			// SBC 0b0000 10rd dddd rrrr
			this.PC++;
		}

		protected virtual void SBCI()
		{
			// SBCI 0b0100 KKKK dddd KKKK
			int d = ((this.Instruction & 0x00F0) >> 3) | 0x1;
			byte K = (byte)(((this.Instruction & 0x0F00) >> 4) | (this.Instruction & 0x000F));
			byte Rd = this.R[d];

			this.R[d] = (byte)(this.R[d] - K - (this.SREG & 0x01));

			this.SREG = (byte)((this.SREG & 0xC0)
				| ((((~Rd & 0x08) & (K & 0x08)) | ((K & 0x08) & (this.R[d] & 0x08)) | ((this.R[d] & 0x08) & (~Rd & 0x08))) << 2)// H
																																// S = N ⊕ V
				| ((((Rd & 0x80) & (~K & 0x80) & (~this.R[d] & 0x80)) | ((~Rd & 0x80) & (K & 0x80) & (this.R[d] & 0x80))) >> 4)	// V
				| ((this.R[d] & 0x80) >> 5)																						// N
				| ((this.R[d] == 0) ? (this.SREG & 0x02) : 0x00)																// Z
				| ((K > Rd) ? 0x01 : 0x00));																					// C

			// S = N ⊕ V
			this.SREG = (byte)((this.SREG & 0xEF) | ((((this.SREG & 0x04) >> 2) ^ ((this.SREG & 0x08) >> 3)) << 4));

			this.PC++;
		}

		protected virtual void SBI()
		{
			// SBI 0b1001 1010 AAAA Abbb
			this.IO[(this.Instruction & 0x00F8) >> 3] |= (byte)(1 << (this.Instruction & 0x0007));
			this.PC++;
		}

		protected virtual void SBIC()
		{
			// SBIC 0b1001 1001 AAAA Abbb
			
		}

		protected virtual void SBIS()
		{
			// SBIS 0b1001 1011 AAAA Abbb

		}

		protected virtual void SBIW()
		{
			// SBIW 0b1001 0111 KKdd KKKK
			this.PC++;
		}

		// SBR (See ORI)

		protected virtual void SBRC()
		{
			// SBRC 0b1111 110r rrrr 0bbb
			
		}

		protected virtual void SBRS()
		{
			// SBRS 0b1111 111r rrrr 0bbb

		}

		// SEC (See BSET)

		// SEH (See BSET)

		// SEI (See BSET)

		// SEN (See BSET)

		// SER (See LDI)

		// SES (See BSET)

		// SET (See BSET)

		// SEV (See BSET)

		// SEZ (See BSET)

		protected virtual void SLEEP()
		{
			// SLEEP 0b1001 0101 1000 1000
			this.PC++;
		}

		protected virtual void SPM()
		{
			// SPM 0b1001 0101 1110 1000
			this.PC++;
		}

		protected virtual void SPM2_1_3()
		{
			// SPM 0b1001 0101 1110 1000
			this.PC++;
		}

		protected virtual void SPM2_4_6()
		{
			// SPM 0b1001 0101 1111 1000
			this.PC++;
		}

		protected virtual void ST_X1()
		{
			// ST 0b1001 001r rrrr 1100
			this.RAM[this.X] = this.R[(this.Instruction & 0x01F0) >> 4];
			this.PC++;
		}

		protected virtual void ST_X2()
		{
			// ST 0b1001 001r rrrr 1101
			this.RAM[this.X++] = this.R[(this.Instruction & 0x01F0) >> 4];
			this.PC++;
		}

		protected virtual void ST_X3()
		{
			// ST 0b1001 001r rrrr 1110
			this.RAM[--this.X] = this.R[(this.Instruction & 0x01F0) >> 4];
			this.PC++;
		}

		// ST_Y1 (See STD_Y)

		protected virtual void ST_Y2()
		{
			// ST 0b1001 001r rrrr 1001
			this.RAM[this.Y++] = this.R[(this.Instruction & 0x01F0) >> 4];
			this.PC++;
		}

		protected virtual void ST_Y3()
		{
			// ST 0b1001 001r rrrr 1010
			this.RAM[--this.Y] = this.R[(this.Instruction & 0x01F0) >> 4];
			this.PC++;
		}

		protected virtual void STD_Y()
		{
			// STD 0b10q0 qq1r rrrr 1qqq
			this.RAM[this.Y + (((this.Instruction & 0x2000) >> 8) | ((this.Instruction & 0x0C00) >> 7) | (this.Instruction & 0x0007))]
				= this.R[(this.Instruction & 0x01F0) >> 4];
			this.PC++;
		}

		// ST_Z1 (See STD_Z)

		protected virtual void ST_Z2()
		{
			// ST 0b1001 001r rrrr 0001
			this.RAM[this.Z++] = this.R[(this.Instruction & 0x01F0) >> 4];
			this.PC++;
		}

		protected virtual void ST_Z3()
		{
			// ST 0b1001 001r rrrr 0010
			this.RAM[--this.Z] = this.R[(this.Instruction & 0x01F0) >> 4];
			this.PC++;
		}

		protected virtual void STD_Z()
		{
			// STD 0b10q0 qq1r rrrr 0qqq
			this.RAM[this.Z + (((this.Instruction & 0x2000) >> 8) | ((this.Instruction & 0x0C00) >> 7) | (this.Instruction & 0x0007))]
				= this.R[(this.Instruction & 0x01F0) >> 4];
			this.PC++;
		}

		protected virtual void STS()
		{
			// STS 0b1001 001d dddd 0000 kkkk kkkk kkkk kkkk
			this.RAM[this.Instruction2] = this.R[(this.Instruction & 0x01F0) >> 4];
			this.PC += 2;
		}

		protected virtual void STS_16bit()
		{
			// STS 0b1010 1kkk dddd kkkk
			this.RAM[((~this.Instruction2 & 0x0100) >> 2) | ((this.Instruction2 & 0x0100) >> 3) | ((this.Instruction2 & 0x0600) >> 5) | (this.Instruction2 & 0x000F)]
				= this.R[((this.Instruction & 0x00F0) >> 4) + 16];
			this.PC++;
		}

		protected virtual void SUB()
		{
			// SUB 0b0001 10rd dddd rrrr
			this.PC++;
		}

		protected virtual void SUBI()
		{
			// SUBI 0b0101 KKKK dddd KKKK
			int d = ((this.Instruction & 0x00F0) >> 3) | 0x1;
			byte K = (byte)(((this.Instruction & 0x0F00) >> 4) | (this.Instruction & 0x000F));
			byte Rd = this.R[d];

			this.R[d] -= K;

			this.SREG = (byte)((this.SREG & 0xC0)
				| ((((~Rd & 0x08) & (K & 0x08)) | ((K & 0x08) & (this.R[d] & 0x08)) | ((this.R[d] & 0x08) & (~Rd & 0x08))) << 2)// H
																																// S = N ⊕ V
				| ((((Rd & 0x80) & (~K & 0x80) & (~this.R[d] & 0x80)) | ((~Rd & 0x80) & (K & 0x80) & (this.R[d] & 0x80))) >> 4)	// V
				| ((this.R[d] & 0x80) >> 5)																						// N
				| ((this.R[d] == 0) ? 0x02 : 0x00)																				// Z
				| ((K > Rd) ? 0x01 : 0x00));																					// C

			// S = N ⊕ V
			this.SREG = (byte)((this.SREG & 0xEF) | ((((this.SREG & 0x04) >> 2) ^ ((this.SREG & 0x08) >> 3)) << 4));

			this.PC++;
		}

		protected virtual void SWAP()
		{
			// SWAP 0b1001 010d dddd 0010
			int d = (this.Instruction & 0x01F0) >> 4;
			this.R[d] = (byte)(((this.R[d] & 0xF0) >> 4) | ((this.R[d] & 0x0F) << 4));
			this.PC++;
		}

		// TST (See AND)

		protected virtual void WDR()
		{
			// WDR 0b1001 0101 1010 1000
			this.PC++;
		}

		protected virtual void XCH()
		{
			// XCH 0b1001 001r rrrr 0100
			byte old = this.RAM[this.Z];
			this.RAM[this.Z] = this.R[(this.Instruction & 0x01F0) >> 4];
			this.R[(this.Instruction & 0x01F0) >> 4] = old;
			this.PC++;
		}
		#endregion
	}
}