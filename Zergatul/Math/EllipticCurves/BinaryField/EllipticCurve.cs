using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Math.EllipticCurves.BinaryField
{
    /// <summary>
    /// y² + xy = x³ + ax² + b in F(2ᵐ)
    /// </summary>
    public class EllipticCurve
    {
        public BigInteger a;
        public BigInteger b;
        public BinaryPolynomial f;
        public ECPoint g;
        public int m;
        public BigInteger n;
        public int h;

        public EllipticCurve()
        {

        }

        public EllipticCurve(BigInteger a, BigInteger b, BinaryPolynomial f, ECPoint g, int m, BigInteger n, int h)
        {
            this.a = a;
            this.b = b;
            this.f = f;
            this.m = m;
            this.n = n;
            this.h = h;

            this.g = g;
            this.g.Curve = this;
        }

        #region Curves

        public static EllipticCurve sect163k1 = new EllipticCurve(
            a: BigInteger.One,
            b: BigInteger.One,
            f: BinaryPolynomial.FromPowers(163, 7, 6, 3, 0),
            g: new ECPoint
            {
                x = new BigInteger(new uint[] { 0x02, 0xFE13C053, 0x7BBC11AC, 0xAA07D793, 0xDE4E6D5E, 0x5C94EEE8 }, ByteOrder.BigEndian),
                y = new BigInteger(new uint[] { 0x02, 0x89070FB0, 0x5D38FF58, 0x321F2E80, 0x0536D538, 0xCCDAA3D9 }, ByteOrder.BigEndian),
            },
            m: 163,
            n: new BigInteger(new uint[] { 0x04, 0x00000000, 0x00000000, 0x00020108, 0xA2E0CC0D, 0x99F8A5EF }, ByteOrder.BigEndian),
            h: 2);

        public static EllipticCurve sect163r1 = new EllipticCurve(
            a: new BigInteger(new uint[] { 0x07, 0xB6882CAA, 0xEFA84F95, 0x54FF8428, 0xBD88E246, 0xD2782AE2 }, ByteOrder.BigEndian),
            b: new BigInteger(new uint[] { 0x07, 0x13612DCD, 0xDCB40AAB, 0x946BDA29, 0xCA91F73A, 0xF958AFD9 }, ByteOrder.BigEndian),
            f: BinaryPolynomial.FromPowers(163, 7, 6, 3, 0),
            g: new ECPoint
            {
                x = new BigInteger(new uint[] { 0x03, 0x69979697, 0xAB438977, 0x89566789, 0x567F787A, 0x7876A654 }, ByteOrder.BigEndian),
                y = new BigInteger(new uint[] { 0x00, 0x435EDB42, 0xEFAFB298, 0x9D51FEFC, 0xE3C80988, 0xF41FF883 }, ByteOrder.BigEndian),
            },
            m: 163,
            n: new BigInteger(new uint[] { 0x03, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFF48AA, 0xB689C29C, 0xA710279B }, ByteOrder.BigEndian),
            h: 2);

        public static EllipticCurve sect163r2 = new EllipticCurve(
            a: BigInteger.One,
            b: new BigInteger(new uint[] { 0x02, 0x0A601907, 0xB8C953CA, 0x1481EB10, 0x512F7874, 0x4A3205FD }, ByteOrder.BigEndian),
            f: BinaryPolynomial.FromPowers(163, 7, 6, 3, 0),
            g: new ECPoint
            {
                x = new BigInteger(new uint[] { 0x03, 0xF0EBA162, 0x86A2D57E, 0xA0991168, 0xD4994637, 0xE8343E36 }, ByteOrder.BigEndian),
                y = new BigInteger(new uint[] { 0x00, 0xD51FBC6C, 0x71A0094F, 0xA2CDD545, 0xB11C5C0C, 0x797324F1 }, ByteOrder.BigEndian),
            },
            m: 163,
            n: new BigInteger(new uint[] { 0x04, 0x00000000, 0x00000000, 0x000292FE, 0x77E70C12, 0xA4234C33 }, ByteOrder.BigEndian),
            h: 2);

        public static EllipticCurve sect233k1 = new EllipticCurve(
            a: BigInteger.Zero,
            b: BigInteger.One,
            f: BinaryPolynomial.FromPowers(233, 74, 0),
            g: new ECPoint
            {
                x = new BigInteger(new uint[] { 0x0172, 0x32BA853A, 0x7E731AF1, 0x29F22FF4, 0x149563A4, 0x19C26BF5, 0x0A4C9D6E, 0xEFAD6126 }, ByteOrder.BigEndian),
                y = new BigInteger(new uint[] { 0x01DB, 0x537DECE8, 0x19B7F70F, 0x555A67C4, 0x27A8CD9B, 0xF18AEB9B, 0x56E0C110, 0x56FAE6A3 }, ByteOrder.BigEndian),
            },
            m: 233,
            n: new BigInteger(new uint[] { 0x80, 0x00000000, 0x00000000, 0x00000000, 0x00069D5B, 0xB915BCD4, 0x6EFB1AD5, 0xF173ABDF }, ByteOrder.BigEndian),
            h: 4);

        public static EllipticCurve sect233r1 = new EllipticCurve(
            a: BigInteger.One,
            b: new BigInteger(new uint[] { 0x0066, 0x647EDE6C, 0x332C7F8C, 0x0923BB58, 0x213B333B, 0x20E9CE42, 0x81FE115F, 0x7D8F90AD }, ByteOrder.BigEndian),
            f: BinaryPolynomial.FromPowers(233, 74, 0),
            g: new ECPoint
            {
                x = new BigInteger(new uint[] { 0x00FA, 0xC9DFCBAC, 0x8313BB21, 0x39F1BB75, 0x5FEF65BC, 0x391F8B36, 0xF8F8EB73, 0x71FD558B }, ByteOrder.BigEndian),
                y = new BigInteger(new uint[] { 0x0100, 0x6A08A419, 0x03350678, 0xE58528BE, 0xBF8A0BEF, 0xF867A7CA, 0x36716F7E, 0x01F81052 }, ByteOrder.BigEndian),
            },
            m: 233,
            n: new BigInteger(new uint[] { 0x0100, 0x00000000, 0x00000000, 0x00000000, 0x0013E974, 0xE72F8A69, 0x22031D26, 0x03CFE0D7 }, ByteOrder.BigEndian),
            h: 2);

        public static EllipticCurve sect239k1 = new EllipticCurve(
            a: BigInteger.Zero,
            b: BigInteger.One,
            f: BinaryPolynomial.FromPowers(239, 158, 0),
            g: new ECPoint
            {
                x = new BigInteger(new uint[] { 0x29A0, 0xB6A887A9, 0x83E97309, 0x88A68727, 0xA8B2D126, 0xC44CC2CC, 0x7B2A6555, 0x193035DC }, ByteOrder.BigEndian),
                y = new BigInteger(new uint[] { 0x7631, 0x0804F12E, 0x549BDB01, 0x1C103089, 0xE73510AC, 0xB275FC31, 0x2A5DC6B7, 0x6553F0CA }, ByteOrder.BigEndian),
            },
            m: 239,
            n: new BigInteger(new uint[] { 0x2000, 0x00000000, 0x00000000, 0x00000000, 0x005A79FE, 0xC67CB6E9, 0x1F1C1DA8, 0x00E478A5 }, ByteOrder.BigEndian),
            h: 4);

        public static EllipticCurve sect283k1 = new EllipticCurve(
            a: BigInteger.Zero,
            b: BigInteger.One,
            f: BinaryPolynomial.FromPowers(283, 12, 7, 5, 0),
            g: new ECPoint
            {
                x = new BigInteger(new uint[] { 0x0503213F, 0x78CA4488, 0x3F1A3B81, 0x62F188E5, 0x53CD265F, 0x23C1567A, 0x16876913, 0xB0C2AC24, 0x58492836 }, ByteOrder.BigEndian),
                y = new BigInteger(new uint[] { 0x01CCDA38, 0x0F1C9E31, 0x8D90F95D, 0x07E5426F, 0xE87E45C0, 0xE8184698, 0xE4596236, 0x4E341161, 0x77DD2259 }, ByteOrder.BigEndian),
            },
            m: 283,
            n: new BigInteger(new uint[] { 0x01FFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFE9AE, 0x2ED07577, 0x265DFF7F, 0x94451E06, 0x1E163C61 }, ByteOrder.BigEndian),
            h: 4);

        public static EllipticCurve sect283r1 = new EllipticCurve(
            a: BigInteger.One,
            b: new BigInteger(new uint[] { 0x027B680A, 0xC8B8596D, 0xA5A4AF8A, 0x19A0303F, 0xCA97FD76, 0x45309FA2, 0xA581485A, 0xF6263E31, 0x3B79A2F5 }, ByteOrder.BigEndian),
            f: BinaryPolynomial.FromPowers(283, 12, 7, 5, 0),
            g: new ECPoint
            {
                x = new BigInteger(new uint[] { 0x05F93925, 0x8DB7DD90, 0xE1934F8C, 0x70B0DFEC, 0x2EED25B8, 0x557EAC9C, 0x80E2E198, 0xF8CDBECD, 0x86B12053 }, ByteOrder.BigEndian),
                y = new BigInteger(new uint[] { 0x03676854, 0xFE24141C, 0xB98FE6D4, 0xB20D02B4, 0x516FF702, 0x350EDDB0, 0x826779C8, 0x13F0DF45, 0xBE8112F4 }, ByteOrder.BigEndian),
            },
            m: 283,
            n: new BigInteger(new uint[] { 0x03FFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFEF90, 0x399660FC, 0x938A9016, 0x5B042A7C, 0xEFADB307 }, ByteOrder.BigEndian),
            h: 2);

        public static EllipticCurve sect409k1 = new EllipticCurve(
            a: BigInteger.Zero,
            b: BigInteger.One,
            f: BinaryPolynomial.FromPowers(409, 87, 0),
            g: new ECPoint
            {
                x = new BigInteger(new uint[] { 0x0060F05F, 0x658F49C1, 0xAD3AB189, 0x0F718421, 0x0EFD0987, 0xE307C84C, 0x27ACCFB8, 0xF9F67CC2, 0xC460189E, 0xB5AAAA62, 0xEE222EB1, 0xB35540CF, 0xE9023746 }, ByteOrder.BigEndian),
                y = new BigInteger(new uint[] { 0x01E36905, 0x0B7C4E42, 0xACBA1DAC, 0xBF04299C, 0x3460782F, 0x918EA427, 0xE6325165, 0xE9EA10E3, 0xDA5F6C42, 0xE9C55215, 0xAA9CA27A, 0x5863EC48, 0xD8E0286B }, ByteOrder.BigEndian),
            },
            m: 409,
            n: new BigInteger(new uint[] { 0x7FFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFE5F, 0x83B2D4EA, 0x20400EC4, 0x557D5ED3, 0xE3E7CA5B, 0x4B5C83B8, 0xE01E5FCF }, ByteOrder.BigEndian),
            h: 4);

        public static EllipticCurve sect409r1 = new EllipticCurve(
            a: BigInteger.One,
            b: new BigInteger(new uint[] { 0x0021A5C2, 0xC8EE9FEB, 0x5C4B9A75, 0x3B7B476B, 0x7FD6422E, 0xF1F3DD67, 0x4761FA99, 0xD6AC27C8, 0xA9A197B2, 0x72822F6C, 0xD57A55AA, 0x4F50AE31, 0x7B13545F }, ByteOrder.BigEndian),
            f: BinaryPolynomial.FromPowers(409, 87, 0),
            g: new ECPoint
            {
                x = new BigInteger(new uint[] { 0x015D4860, 0xD088DDB3, 0x496B0C60, 0x64756260, 0x441CDE4A, 0xF1771D4D, 0xB01FFE5B, 0x34E59703, 0xDC255A86, 0x8A118051, 0x5603AEAB, 0x60794E54, 0xBB7996A7 }, ByteOrder.BigEndian),
                y = new BigInteger(new uint[] { 0x0061B1CF, 0xAB6BE5F3, 0x2BBFA783, 0x24ED106A, 0x7636B9C5, 0xA7BD198D, 0x0158AA4F, 0x5488D08F, 0x38514F1F, 0xDF4B4F40, 0xD2181B36, 0x81C364BA, 0x0273C706 }, ByteOrder.BigEndian),
            },
            m: 409,
            n: new BigInteger(new uint[] { 0x01000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x000001E2, 0xAAD6A612, 0xF33307BE, 0x5FA47C3C, 0x9E052F83, 0x8164CD37, 0xD9A21173 }, ByteOrder.BigEndian),
            h: 2);

        public static EllipticCurve sect571k1 = new EllipticCurve(
            a: BigInteger.Zero,
            b: BigInteger.One,
            f: BinaryPolynomial.FromPowers(571, 10, 5, 2, 0),
            g: new ECPoint
            {
                x = new BigInteger(new uint[] { 0x026EB7A8, 0x59923FBC, 0x82189631, 0xF8103FE4, 0xAC9CA297, 0x0012D5D4, 0x60248048, 0x01841CA4, 0x43709584, 0x93B205E6, 0x47DA304D, 0xB4CEB08C, 0xBBD1BA39, 0x494776FB, 0x988B4717, 0x4DCA88C7, 0xE2945283, 0xA01C8972 }, ByteOrder.BigEndian),
                y = new BigInteger(new uint[] { 0x0349DC80, 0x7F4FBF37, 0x4F4AEADE, 0x3BCA9531, 0x4DD58CEC, 0x9F307A54, 0xFFC61EFC, 0x006D8A2C, 0x9D4979C0, 0xAC44AEA7, 0x4FBEBBB9, 0xF772AEDC, 0xB620B01A, 0x7BA7AF1B, 0x320430C8, 0x591984F6, 0x01CD4C14, 0x3EF1C7A3 }, ByteOrder.BigEndian),
            },
            m: 571,
            n: new BigInteger(new uint[] { 0x02000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x131850E1, 0xF19A63E4, 0xB391A8DB, 0x917F4138, 0xB630D84B, 0xE5D63938, 0x1E91DEB4, 0x5CFE778F, 0x637C1001 }, ByteOrder.BigEndian),
            h: 4);

        public static EllipticCurve sect571r1 = new EllipticCurve(
            a: BigInteger.One,
            b: new BigInteger(new uint[] { 0x02F40E7E, 0x2221F295, 0xDE297117, 0xB7F3D62F, 0x5C6A97FF, 0xCB8CEFF1, 0xCD6BA8CE, 0x4A9A18AD, 0x84FFABBD, 0x8EFA5933, 0x2BE7AD67, 0x56A66E29, 0x4AFD185A, 0x78FF12AA, 0x520E4DE7, 0x39BACA0C, 0x7FFEFF7F, 0x2955727A }, ByteOrder.BigEndian),
            f: BinaryPolynomial.FromPowers(571, 10, 5, 2, 0),
            g: new ECPoint
            {
                x = new BigInteger(new uint[] { 0x0303001D, 0x34B85629, 0x6C16C0D4, 0x0D3CD775, 0x0A93D1D2, 0x955FA80A, 0xA5F40FC8, 0xDB7B2ABD, 0xBDE53950, 0xF4C0D293, 0xCDD711A3, 0x5B67FB14, 0x99AE6003, 0x8614F139, 0x4ABFA3B4, 0xC850D927, 0xE1E7769C, 0x8EEC2D19 }, ByteOrder.BigEndian),
                y = new BigInteger(new uint[] { 0x037BF273, 0x42DA639B, 0x6DCCFFFE, 0xB73D69D7, 0x8C6C27A6, 0x009CBBCA, 0x1980F853, 0x3921E8A6, 0x84423E43, 0xBAB08A57, 0x6291AF8F, 0x461BB2A8, 0xB3531D2F, 0x0485C19B, 0x16E2F151, 0x6E23DD3C, 0x1A4827AF, 0x1B8AC15B }, ByteOrder.BigEndian),
            },
            m: 571,
            n: new BigInteger(new uint[] { 0x03FFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xFFFFFFFF, 0xE661CE18, 0xFF559873, 0x08059B18, 0x6823851E, 0xC7DD9CA1, 0x161DE93D, 0x5174D66E, 0x8382E9BB, 0x2FE84E47 }, ByteOrder.BigEndian),
            h: 2);

        #endregion
    }
}