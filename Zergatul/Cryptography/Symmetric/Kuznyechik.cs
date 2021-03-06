﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;

namespace Zergatul.Cryptography.Symmetric
{
    /// <summary>
    /// GOST R 34.12-2015
    /// </summary>
    public class Kuznyechik : AbstractBlockCipher
    {
        public override int BlockSize => 16;
        public override int KeySize => 32;

        #region Arrays with constants

        private static readonly byte[] Pi = new byte[]
        {
            0xFC, 0xEE, 0xDD, 0x11, 0xCF, 0x6E, 0x31, 0x16, 0xFB, 0xC4, 0xFA, 0xDA, 0x23, 0xC5, 0x04, 0x4D,
            0xE9, 0x77, 0xF0, 0xDB, 0x93, 0x2E, 0x99, 0xBA, 0x17, 0x36, 0xF1, 0xBB, 0x14, 0xCD, 0x5F, 0xC1,
            0xF9, 0x18, 0x65, 0x5A, 0xE2, 0x5C, 0xEF, 0x21, 0x81, 0x1C, 0x3C, 0x42, 0x8B, 0x01, 0x8E, 0x4F,
            0x05, 0x84, 0x02, 0xAE, 0xE3, 0x6A, 0x8F, 0xA0, 0x06, 0x0B, 0xED, 0x98, 0x7F, 0xD4, 0xD3, 0x1F,
            0xEB, 0x34, 0x2C, 0x51, 0xEA, 0xC8, 0x48, 0xAB, 0xF2, 0x2A, 0x68, 0xA2, 0xFD, 0x3A, 0xCE, 0xCC,
            0xB5, 0x70, 0x0E, 0x56, 0x08, 0x0C, 0x76, 0x12, 0xBF, 0x72, 0x13, 0x47, 0x9C, 0xB7, 0x5D, 0x87,
            0x15, 0xA1, 0x96, 0x29, 0x10, 0x7B, 0x9A, 0xC7, 0xF3, 0x91, 0x78, 0x6F, 0x9D, 0x9E, 0xB2, 0xB1,
            0x32, 0x75, 0x19, 0x3D, 0xFF, 0x35, 0x8A, 0x7E, 0x6D, 0x54, 0xC6, 0x80, 0xC3, 0xBD, 0x0D, 0x57,
            0xDF, 0xF5, 0x24, 0xA9, 0x3E, 0xA8, 0x43, 0xC9, 0xD7, 0x79, 0xD6, 0xF6, 0x7C, 0x22, 0xB9, 0x03,
            0xE0, 0x0F, 0xEC, 0xDE, 0x7A, 0x94, 0xB0, 0xBC, 0xDC, 0xE8, 0x28, 0x50, 0x4E, 0x33, 0x0A, 0x4A,
            0xA7, 0x97, 0x60, 0x73, 0x1E, 0x00, 0x62, 0x44, 0x1A, 0xB8, 0x38, 0x82, 0x64, 0x9F, 0x26, 0x41,
            0xAD, 0x45, 0x46, 0x92, 0x27, 0x5E, 0x55, 0x2F, 0x8C, 0xA3, 0xA5, 0x7D, 0x69, 0xD5, 0x95, 0x3B,
            0x07, 0x58, 0xB3, 0x40, 0x86, 0xAC, 0x1D, 0xF7, 0x30, 0x37, 0x6B, 0xE4, 0x88, 0xD9, 0xE7, 0x89,
            0xE1, 0x1B, 0x83, 0x49, 0x4C, 0x3F, 0xF8, 0xFE, 0x8D, 0x53, 0xAA, 0x90, 0xCA, 0xD8, 0x85, 0x61,
            0x20, 0x71, 0x67, 0xA4, 0x2D, 0x2B, 0x09, 0x5B, 0xCB, 0x9B, 0x25, 0xD0, 0xBE, 0xE5, 0x6C, 0x52,
            0x59, 0xA6, 0x74, 0xD2, 0xE6, 0xF4, 0xB4, 0xC0, 0xD1, 0x66, 0xAF, 0xC2, 0x39, 0x4B, 0x63, 0xB6,
        };

        private static readonly byte[] PiInv = new byte[]
        {
            0xA5, 0x2D, 0x32, 0x8F, 0x0E, 0x30, 0x38, 0xC0, 0x54, 0xE6, 0x9E, 0x39, 0x55, 0x7E, 0x52, 0x91,
            0x64, 0x03, 0x57, 0x5A, 0x1C, 0x60, 0x07, 0x18, 0x21, 0x72, 0xA8, 0xD1, 0x29, 0xC6, 0xA4, 0x3F,
            0xE0, 0x27, 0x8D, 0x0C, 0x82, 0xEA, 0xAE, 0xB4, 0x9A, 0x63, 0x49, 0xE5, 0x42, 0xE4, 0x15, 0xB7,
            0xC8, 0x06, 0x70, 0x9D, 0x41, 0x75, 0x19, 0xC9, 0xAA, 0xFC, 0x4D, 0xBF, 0x2A, 0x73, 0x84, 0xD5,
            0xC3, 0xAF, 0x2B, 0x86, 0xA7, 0xB1, 0xB2, 0x5B, 0x46, 0xD3, 0x9F, 0xFD, 0xD4, 0x0F, 0x9C, 0x2F,
            0x9B, 0x43, 0xEF, 0xD9, 0x79, 0xB6, 0x53, 0x7F, 0xC1, 0xF0, 0x23, 0xE7, 0x25, 0x5E, 0xB5, 0x1E,
            0xA2, 0xDF, 0xA6, 0xFE, 0xAC, 0x22, 0xF9, 0xE2, 0x4A, 0xBC, 0x35, 0xCA, 0xEE, 0x78, 0x05, 0x6B,
            0x51, 0xE1, 0x59, 0xA3, 0xF2, 0x71, 0x56, 0x11, 0x6A, 0x89, 0x94, 0x65, 0x8C, 0xBB, 0x77, 0x3C,
            0x7B, 0x28, 0xAB, 0xD2, 0x31, 0xDE, 0xC4, 0x5F, 0xCC, 0xCF, 0x76, 0x2C, 0xB8, 0xD8, 0x2E, 0x36,
            0xDB, 0x69, 0xB3, 0x14, 0x95, 0xBE, 0x62, 0xA1, 0x3B, 0x16, 0x66, 0xE9, 0x5C, 0x6C, 0x6D, 0xAD,
            0x37, 0x61, 0x4B, 0xB9, 0xE3, 0xBA, 0xF1, 0xA0, 0x85, 0x83, 0xDA, 0x47, 0xC5, 0xB0, 0x33, 0xFA,
            0x96, 0x6F, 0x6E, 0xC2, 0xF6, 0x50, 0xFF, 0x5D, 0xA9, 0x8E, 0x17, 0x1B, 0x97, 0x7D, 0xEC, 0x58,
            0xF7, 0x1F, 0xFB, 0x7C, 0x09, 0x0D, 0x7A, 0x67, 0x45, 0x87, 0xDC, 0xE8, 0x4F, 0x1D, 0x4E, 0x04,
            0xEB, 0xF8, 0xF3, 0x3E, 0x3D, 0xBD, 0x8A, 0x88, 0xDD, 0xCD, 0x0B, 0x13, 0x98, 0x02, 0x93, 0x80,
            0x90, 0xD0, 0x24, 0x34, 0xCB, 0xED, 0xF4, 0xCE, 0x99, 0x10, 0x44, 0x40, 0x92, 0x3A, 0x01, 0x26,
            0x12, 0x1A, 0x48, 0x68, 0xF5, 0x81, 0x8B, 0xC7, 0xD6, 0x20, 0x0A, 0x08, 0x00, 0x4C, 0xD7, 0x74,
        };

        private static readonly byte[][] C = new byte[][]
        {
            new byte[] { 0x6E, 0xA2, 0x76, 0x72, 0x6C, 0x48, 0x7A, 0xB8, 0x5D, 0x27, 0xBD, 0x10, 0xDD, 0x84, 0x94, 0x01 },
            new byte[] { 0xDC, 0x87, 0xEC, 0xE4, 0xD8, 0x90, 0xF4, 0xB3, 0xBA, 0x4E, 0xB9, 0x20, 0x79, 0xCB, 0xEB, 0x02 },
            new byte[] { 0xB2, 0x25, 0x9A, 0x96, 0xB4, 0xD8, 0x8E, 0x0B, 0xE7, 0x69, 0x04, 0x30, 0xA4, 0x4F, 0x7F, 0x03 },
            new byte[] { 0x7B, 0xCD, 0x1B, 0x0B, 0x73, 0xE3, 0x2B, 0xA5, 0xB7, 0x9C, 0xB1, 0x40, 0xF2, 0x55, 0x15, 0x04 },
            new byte[] { 0x15, 0x6F, 0x6D, 0x79, 0x1F, 0xAB, 0x51, 0x1D, 0xEA, 0xBB, 0x0C, 0x50, 0x2F, 0xD1, 0x81, 0x05 },
            new byte[] { 0xA7, 0x4A, 0xF7, 0xEF, 0xAB, 0x73, 0xDF, 0x16, 0x0D, 0xD2, 0x08, 0x60, 0x8B, 0x9E, 0xFE, 0x06 },
            new byte[] { 0xC9, 0xE8, 0x81, 0x9D, 0xC7, 0x3B, 0xA5, 0xAE, 0x50, 0xF5, 0xB5, 0x70, 0x56, 0x1A, 0x6A, 0x07 },
            new byte[] { 0xF6, 0x59, 0x36, 0x16, 0xE6, 0x05, 0x56, 0x89, 0xAD, 0xFB, 0xA1, 0x80, 0x27, 0xAA, 0x2A, 0x08 },
            new byte[] { 0x98, 0xFB, 0x40, 0x64, 0x8A, 0x4D, 0x2C, 0x31, 0xF0, 0xDC, 0x1C, 0x90, 0xFA, 0x2E, 0xBE, 0x09 },
            new byte[] { 0x2A, 0xDE, 0xDA, 0xF2, 0x3E, 0x95, 0xA2, 0x3A, 0x17, 0xB5, 0x18, 0xA0, 0x5E, 0x61, 0xC1, 0x0A },
            new byte[] { 0x44, 0x7C, 0xAC, 0x80, 0x52, 0xDD, 0xD8, 0x82, 0x4A, 0x92, 0xA5, 0xB0, 0x83, 0xE5, 0x55, 0x0B },
            new byte[] { 0x8D, 0x94, 0x2D, 0x1D, 0x95, 0xE6, 0x7D, 0x2C, 0x1A, 0x67, 0x10, 0xC0, 0xD5, 0xFF, 0x3F, 0x0C },
            new byte[] { 0xE3, 0x36, 0x5B, 0x6F, 0xF9, 0xAE, 0x07, 0x94, 0x47, 0x40, 0xAD, 0xD0, 0x08, 0x7B, 0xAB, 0x0D },
            new byte[] { 0x51, 0x13, 0xC1, 0xF9, 0x4D, 0x76, 0x89, 0x9F, 0xA0, 0x29, 0xA9, 0xE0, 0xAC, 0x34, 0xD4, 0x0E },
            new byte[] { 0x3F, 0xB1, 0xB7, 0x8B, 0x21, 0x3E, 0xF3, 0x27, 0xFD, 0x0E, 0x14, 0xF0, 0x71, 0xB0, 0x40, 0x0F },
            new byte[] { 0x2F, 0xB2, 0x6C, 0x2C, 0x0F, 0x0A, 0xAC, 0xD1, 0x99, 0x35, 0x81, 0xC3, 0x4E, 0x97, 0x54, 0x10 },
            new byte[] { 0x41, 0x10, 0x1A, 0x5E, 0x63, 0x42, 0xD6, 0x69, 0xC4, 0x12, 0x3C, 0xD3, 0x93, 0x13, 0xC0, 0x11 },
            new byte[] { 0xF3, 0x35, 0x80, 0xC8, 0xD7, 0x9A, 0x58, 0x62, 0x23, 0x7B, 0x38, 0xE3, 0x37, 0x5C, 0xBF, 0x12 },
            new byte[] { 0x9D, 0x97, 0xF6, 0xBA, 0xBB, 0xD2, 0x22, 0xDA, 0x7E, 0x5C, 0x85, 0xF3, 0xEA, 0xD8, 0x2B, 0x13 },
            new byte[] { 0x54, 0x7F, 0x77, 0x27, 0x7C, 0xE9, 0x87, 0x74, 0x2E, 0xA9, 0x30, 0x83, 0xBC, 0xC2, 0x41, 0x14 },
            new byte[] { 0x3A, 0xDD, 0x01, 0x55, 0x10, 0xA1, 0xFD, 0xCC, 0x73, 0x8E, 0x8D, 0x93, 0x61, 0x46, 0xD5, 0x15 },
            new byte[] { 0x88, 0xF8, 0x9B, 0xC3, 0xA4, 0x79, 0x73, 0xC7, 0x94, 0xE7, 0x89, 0xA3, 0xC5, 0x09, 0xAA, 0x16 },
            new byte[] { 0xE6, 0x5A, 0xED, 0xB1, 0xC8, 0x31, 0x09, 0x7F, 0xC9, 0xC0, 0x34, 0xB3, 0x18, 0x8D, 0x3E, 0x17 },
            new byte[] { 0xD9, 0xEB, 0x5A, 0x3A, 0xE9, 0x0F, 0xFA, 0x58, 0x34, 0xCE, 0x20, 0x43, 0x69, 0x3D, 0x7E, 0x18 },
            new byte[] { 0xB7, 0x49, 0x2C, 0x48, 0x85, 0x47, 0x80, 0xE0, 0x69, 0xE9, 0x9D, 0x53, 0xB4, 0xB9, 0xEA, 0x19 },
            new byte[] { 0x05, 0x6C, 0xB6, 0xDE, 0x31, 0x9F, 0x0E, 0xEB, 0x8E, 0x80, 0x99, 0x63, 0x10, 0xF6, 0x95, 0x1A },
            new byte[] { 0x6B, 0xCE, 0xC0, 0xAC, 0x5D, 0xD7, 0x74, 0x53, 0xD3, 0xA7, 0x24, 0x73, 0xCD, 0x72, 0x01, 0x1B },
            new byte[] { 0xA2, 0x26, 0x41, 0x31, 0x9A, 0xEC, 0xD1, 0xFD, 0x83, 0x52, 0x91, 0x03, 0x9B, 0x68, 0x6B, 0x1C },
            new byte[] { 0xCC, 0x84, 0x37, 0x43, 0xF6, 0xA4, 0xAB, 0x45, 0xDE, 0x75, 0x2C, 0x13, 0x46, 0xEC, 0xFF, 0x1D },
            new byte[] { 0x7E, 0xA1, 0xAD, 0xD5, 0x42, 0x7C, 0x25, 0x4E, 0x39, 0x1C, 0x28, 0x23, 0xE2, 0xA3, 0x80, 0x1E },
            new byte[] { 0x10, 0x03, 0xDB, 0xA7, 0x2E, 0x34, 0x5F, 0xF6, 0x64, 0x3B, 0x95, 0x33, 0x3F, 0x27, 0x14, 0x1F },
            new byte[] { 0x5E, 0xA7, 0xD8, 0x58, 0x1E, 0x14, 0x9B, 0x61, 0xF1, 0x6A, 0xC1, 0x45, 0x9C, 0xED, 0xA8, 0x20 },
        };

        private static readonly byte[] BP016 = new byte[]
        {
            0x00, 0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70, 0x80, 0x90, 0xA0, 0xB0, 0xC0, 0xD0, 0xE0, 0xF0,
            0xC3, 0xD3, 0xE3, 0xF3, 0x83, 0x93, 0xA3, 0xB3, 0x43, 0x53, 0x63, 0x73, 0x03, 0x13, 0x23, 0x33,
            0x45, 0x55, 0x65, 0x75, 0x05, 0x15, 0x25, 0x35, 0xC5, 0xD5, 0xE5, 0xF5, 0x85, 0x95, 0xA5, 0xB5,
            0x86, 0x96, 0xA6, 0xB6, 0xC6, 0xD6, 0xE6, 0xF6, 0x06, 0x16, 0x26, 0x36, 0x46, 0x56, 0x66, 0x76,
            0x8A, 0x9A, 0xAA, 0xBA, 0xCA, 0xDA, 0xEA, 0xFA, 0x0A, 0x1A, 0x2A, 0x3A, 0x4A, 0x5A, 0x6A, 0x7A,
            0x49, 0x59, 0x69, 0x79, 0x09, 0x19, 0x29, 0x39, 0xC9, 0xD9, 0xE9, 0xF9, 0x89, 0x99, 0xA9, 0xB9,
            0xCF, 0xDF, 0xEF, 0xFF, 0x8F, 0x9F, 0xAF, 0xBF, 0x4F, 0x5F, 0x6F, 0x7F, 0x0F, 0x1F, 0x2F, 0x3F,
            0x0C, 0x1C, 0x2C, 0x3C, 0x4C, 0x5C, 0x6C, 0x7C, 0x8C, 0x9C, 0xAC, 0xBC, 0xCC, 0xDC, 0xEC, 0xFC,
            0xD7, 0xC7, 0xF7, 0xE7, 0x97, 0x87, 0xB7, 0xA7, 0x57, 0x47, 0x77, 0x67, 0x17, 0x07, 0x37, 0x27,
            0x14, 0x04, 0x34, 0x24, 0x54, 0x44, 0x74, 0x64, 0x94, 0x84, 0xB4, 0xA4, 0xD4, 0xC4, 0xF4, 0xE4,
            0x92, 0x82, 0xB2, 0xA2, 0xD2, 0xC2, 0xF2, 0xE2, 0x12, 0x02, 0x32, 0x22, 0x52, 0x42, 0x72, 0x62,
            0x51, 0x41, 0x71, 0x61, 0x11, 0x01, 0x31, 0x21, 0xD1, 0xC1, 0xF1, 0xE1, 0x91, 0x81, 0xB1, 0xA1,
            0x5D, 0x4D, 0x7D, 0x6D, 0x1D, 0x0D, 0x3D, 0x2D, 0xDD, 0xCD, 0xFD, 0xED, 0x9D, 0x8D, 0xBD, 0xAD,
            0x9E, 0x8E, 0xBE, 0xAE, 0xDE, 0xCE, 0xFE, 0xEE, 0x1E, 0x0E, 0x3E, 0x2E, 0x5E, 0x4E, 0x7E, 0x6E,
            0x18, 0x08, 0x38, 0x28, 0x58, 0x48, 0x78, 0x68, 0x98, 0x88, 0xB8, 0xA8, 0xD8, 0xC8, 0xF8, 0xE8,
            0xDB, 0xCB, 0xFB, 0xEB, 0x9B, 0x8B, 0xBB, 0xAB, 0x5B, 0x4B, 0x7B, 0x6B, 0x1B, 0x0B, 0x3B, 0x2B,
        };

        private static readonly byte[] BP032 = new byte[]
        {
            0x00, 0x20, 0x40, 0x60, 0x80, 0xA0, 0xC0, 0xE0, 0xC3, 0xE3, 0x83, 0xA3, 0x43, 0x63, 0x03, 0x23,
            0x45, 0x65, 0x05, 0x25, 0xC5, 0xE5, 0x85, 0xA5, 0x86, 0xA6, 0xC6, 0xE6, 0x06, 0x26, 0x46, 0x66,
            0x8A, 0xAA, 0xCA, 0xEA, 0x0A, 0x2A, 0x4A, 0x6A, 0x49, 0x69, 0x09, 0x29, 0xC9, 0xE9, 0x89, 0xA9,
            0xCF, 0xEF, 0x8F, 0xAF, 0x4F, 0x6F, 0x0F, 0x2F, 0x0C, 0x2C, 0x4C, 0x6C, 0x8C, 0xAC, 0xCC, 0xEC,
            0xD7, 0xF7, 0x97, 0xB7, 0x57, 0x77, 0x17, 0x37, 0x14, 0x34, 0x54, 0x74, 0x94, 0xB4, 0xD4, 0xF4,
            0x92, 0xB2, 0xD2, 0xF2, 0x12, 0x32, 0x52, 0x72, 0x51, 0x71, 0x11, 0x31, 0xD1, 0xF1, 0x91, 0xB1,
            0x5D, 0x7D, 0x1D, 0x3D, 0xDD, 0xFD, 0x9D, 0xBD, 0x9E, 0xBE, 0xDE, 0xFE, 0x1E, 0x3E, 0x5E, 0x7E,
            0x18, 0x38, 0x58, 0x78, 0x98, 0xB8, 0xD8, 0xF8, 0xDB, 0xFB, 0x9B, 0xBB, 0x5B, 0x7B, 0x1B, 0x3B,
            0x6D, 0x4D, 0x2D, 0x0D, 0xED, 0xCD, 0xAD, 0x8D, 0xAE, 0x8E, 0xEE, 0xCE, 0x2E, 0x0E, 0x6E, 0x4E,
            0x28, 0x08, 0x68, 0x48, 0xA8, 0x88, 0xE8, 0xC8, 0xEB, 0xCB, 0xAB, 0x8B, 0x6B, 0x4B, 0x2B, 0x0B,
            0xE7, 0xC7, 0xA7, 0x87, 0x67, 0x47, 0x27, 0x07, 0x24, 0x04, 0x64, 0x44, 0xA4, 0x84, 0xE4, 0xC4,
            0xA2, 0x82, 0xE2, 0xC2, 0x22, 0x02, 0x62, 0x42, 0x61, 0x41, 0x21, 0x01, 0xE1, 0xC1, 0xA1, 0x81,
            0xBA, 0x9A, 0xFA, 0xDA, 0x3A, 0x1A, 0x7A, 0x5A, 0x79, 0x59, 0x39, 0x19, 0xF9, 0xD9, 0xB9, 0x99,
            0xFF, 0xDF, 0xBF, 0x9F, 0x7F, 0x5F, 0x3F, 0x1F, 0x3C, 0x1C, 0x7C, 0x5C, 0xBC, 0x9C, 0xFC, 0xDC,
            0x30, 0x10, 0x70, 0x50, 0xB0, 0x90, 0xF0, 0xD0, 0xF3, 0xD3, 0xB3, 0x93, 0x73, 0x53, 0x33, 0x13,
            0x75, 0x55, 0x35, 0x15, 0xF5, 0xD5, 0xB5, 0x95, 0xB6, 0x96, 0xF6, 0xD6, 0x36, 0x16, 0x76, 0x56,
        };

        private static readonly byte[] BP133 = new byte[]
        {
            0x00, 0x85, 0xC9, 0x4C, 0x51, 0xD4, 0x98, 0x1D, 0xA2, 0x27, 0x6B, 0xEE, 0xF3, 0x76, 0x3A, 0xBF,
            0x87, 0x02, 0x4E, 0xCB, 0xD6, 0x53, 0x1F, 0x9A, 0x25, 0xA0, 0xEC, 0x69, 0x74, 0xF1, 0xBD, 0x38,
            0xCD, 0x48, 0x04, 0x81, 0x9C, 0x19, 0x55, 0xD0, 0x6F, 0xEA, 0xA6, 0x23, 0x3E, 0xBB, 0xF7, 0x72,
            0x4A, 0xCF, 0x83, 0x06, 0x1B, 0x9E, 0xD2, 0x57, 0xE8, 0x6D, 0x21, 0xA4, 0xB9, 0x3C, 0x70, 0xF5,
            0x59, 0xDC, 0x90, 0x15, 0x08, 0x8D, 0xC1, 0x44, 0xFB, 0x7E, 0x32, 0xB7, 0xAA, 0x2F, 0x63, 0xE6,
            0xDE, 0x5B, 0x17, 0x92, 0x8F, 0x0A, 0x46, 0xC3, 0x7C, 0xF9, 0xB5, 0x30, 0x2D, 0xA8, 0xE4, 0x61,
            0x94, 0x11, 0x5D, 0xD8, 0xC5, 0x40, 0x0C, 0x89, 0x36, 0xB3, 0xFF, 0x7A, 0x67, 0xE2, 0xAE, 0x2B,
            0x13, 0x96, 0xDA, 0x5F, 0x42, 0xC7, 0x8B, 0x0E, 0xB1, 0x34, 0x78, 0xFD, 0xE0, 0x65, 0x29, 0xAC,
            0xB2, 0x37, 0x7B, 0xFE, 0xE3, 0x66, 0x2A, 0xAF, 0x10, 0x95, 0xD9, 0x5C, 0x41, 0xC4, 0x88, 0x0D,
            0x35, 0xB0, 0xFC, 0x79, 0x64, 0xE1, 0xAD, 0x28, 0x97, 0x12, 0x5E, 0xDB, 0xC6, 0x43, 0x0F, 0x8A,
            0x7F, 0xFA, 0xB6, 0x33, 0x2E, 0xAB, 0xE7, 0x62, 0xDD, 0x58, 0x14, 0x91, 0x8C, 0x09, 0x45, 0xC0,
            0xF8, 0x7D, 0x31, 0xB4, 0xA9, 0x2C, 0x60, 0xE5, 0x5A, 0xDF, 0x93, 0x16, 0x0B, 0x8E, 0xC2, 0x47,
            0xEB, 0x6E, 0x22, 0xA7, 0xBA, 0x3F, 0x73, 0xF6, 0x49, 0xCC, 0x80, 0x05, 0x18, 0x9D, 0xD1, 0x54,
            0x6C, 0xE9, 0xA5, 0x20, 0x3D, 0xB8, 0xF4, 0x71, 0xCE, 0x4B, 0x07, 0x82, 0x9F, 0x1A, 0x56, 0xD3,
            0x26, 0xA3, 0xEF, 0x6A, 0x77, 0xF2, 0xBE, 0x3B, 0x84, 0x01, 0x4D, 0xC8, 0xD5, 0x50, 0x1C, 0x99,
            0xA1, 0x24, 0x68, 0xED, 0xF0, 0x75, 0x39, 0xBC, 0x03, 0x86, 0xCA, 0x4F, 0x52, 0xD7, 0x9B, 0x1E,
        };

        private static readonly byte[] BP148 = new byte[]
        {
            0x00, 0x94, 0xEB, 0x7F, 0x15, 0x81, 0xFE, 0x6A, 0x2A, 0xBE, 0xC1, 0x55, 0x3F, 0xAB, 0xD4, 0x40,
            0x54, 0xC0, 0xBF, 0x2B, 0x41, 0xD5, 0xAA, 0x3E, 0x7E, 0xEA, 0x95, 0x01, 0x6B, 0xFF, 0x80, 0x14,
            0xA8, 0x3C, 0x43, 0xD7, 0xBD, 0x29, 0x56, 0xC2, 0x82, 0x16, 0x69, 0xFD, 0x97, 0x03, 0x7C, 0xE8,
            0xFC, 0x68, 0x17, 0x83, 0xE9, 0x7D, 0x02, 0x96, 0xD6, 0x42, 0x3D, 0xA9, 0xC3, 0x57, 0x28, 0xBC,
            0x93, 0x07, 0x78, 0xEC, 0x86, 0x12, 0x6D, 0xF9, 0xB9, 0x2D, 0x52, 0xC6, 0xAC, 0x38, 0x47, 0xD3,
            0xC7, 0x53, 0x2C, 0xB8, 0xD2, 0x46, 0x39, 0xAD, 0xED, 0x79, 0x06, 0x92, 0xF8, 0x6C, 0x13, 0x87,
            0x3B, 0xAF, 0xD0, 0x44, 0x2E, 0xBA, 0xC5, 0x51, 0x11, 0x85, 0xFA, 0x6E, 0x04, 0x90, 0xEF, 0x7B,
            0x6F, 0xFB, 0x84, 0x10, 0x7A, 0xEE, 0x91, 0x05, 0x45, 0xD1, 0xAE, 0x3A, 0x50, 0xC4, 0xBB, 0x2F,
            0xE5, 0x71, 0x0E, 0x9A, 0xF0, 0x64, 0x1B, 0x8F, 0xCF, 0x5B, 0x24, 0xB0, 0xDA, 0x4E, 0x31, 0xA5,
            0xB1, 0x25, 0x5A, 0xCE, 0xA4, 0x30, 0x4F, 0xDB, 0x9B, 0x0F, 0x70, 0xE4, 0x8E, 0x1A, 0x65, 0xF1,
            0x4D, 0xD9, 0xA6, 0x32, 0x58, 0xCC, 0xB3, 0x27, 0x67, 0xF3, 0x8C, 0x18, 0x72, 0xE6, 0x99, 0x0D,
            0x19, 0x8D, 0xF2, 0x66, 0x0C, 0x98, 0xE7, 0x73, 0x33, 0xA7, 0xD8, 0x4C, 0x26, 0xB2, 0xCD, 0x59,
            0x76, 0xE2, 0x9D, 0x09, 0x63, 0xF7, 0x88, 0x1C, 0x5C, 0xC8, 0xB7, 0x23, 0x49, 0xDD, 0xA2, 0x36,
            0x22, 0xB6, 0xC9, 0x5D, 0x37, 0xA3, 0xDC, 0x48, 0x08, 0x9C, 0xE3, 0x77, 0x1D, 0x89, 0xF6, 0x62,
            0xDE, 0x4A, 0x35, 0xA1, 0xCB, 0x5F, 0x20, 0xB4, 0xF4, 0x60, 0x1F, 0x8B, 0xE1, 0x75, 0x0A, 0x9E,
            0x8A, 0x1E, 0x61, 0xF5, 0x9F, 0x0B, 0x74, 0xE0, 0xA0, 0x34, 0x4B, 0xDF, 0xB5, 0x21, 0x5E, 0xCA,
        };

        private static readonly byte[] BP192 = new byte[]
        {
            0x00, 0xC0, 0x43, 0x83, 0x86, 0x46, 0xC5, 0x05, 0xCF, 0x0F, 0x8C, 0x4C, 0x49, 0x89, 0x0A, 0xCA,
            0x5D, 0x9D, 0x1E, 0xDE, 0xDB, 0x1B, 0x98, 0x58, 0x92, 0x52, 0xD1, 0x11, 0x14, 0xD4, 0x57, 0x97,
            0xBA, 0x7A, 0xF9, 0x39, 0x3C, 0xFC, 0x7F, 0xBF, 0x75, 0xB5, 0x36, 0xF6, 0xF3, 0x33, 0xB0, 0x70,
            0xE7, 0x27, 0xA4, 0x64, 0x61, 0xA1, 0x22, 0xE2, 0x28, 0xE8, 0x6B, 0xAB, 0xAE, 0x6E, 0xED, 0x2D,
            0xB7, 0x77, 0xF4, 0x34, 0x31, 0xF1, 0x72, 0xB2, 0x78, 0xB8, 0x3B, 0xFB, 0xFE, 0x3E, 0xBD, 0x7D,
            0xEA, 0x2A, 0xA9, 0x69, 0x6C, 0xAC, 0x2F, 0xEF, 0x25, 0xE5, 0x66, 0xA6, 0xA3, 0x63, 0xE0, 0x20,
            0x0D, 0xCD, 0x4E, 0x8E, 0x8B, 0x4B, 0xC8, 0x08, 0xC2, 0x02, 0x81, 0x41, 0x44, 0x84, 0x07, 0xC7,
            0x50, 0x90, 0x13, 0xD3, 0xD6, 0x16, 0x95, 0x55, 0x9F, 0x5F, 0xDC, 0x1C, 0x19, 0xD9, 0x5A, 0x9A,
            0xAD, 0x6D, 0xEE, 0x2E, 0x2B, 0xEB, 0x68, 0xA8, 0x62, 0xA2, 0x21, 0xE1, 0xE4, 0x24, 0xA7, 0x67,
            0xF0, 0x30, 0xB3, 0x73, 0x76, 0xB6, 0x35, 0xF5, 0x3F, 0xFF, 0x7C, 0xBC, 0xB9, 0x79, 0xFA, 0x3A,
            0x17, 0xD7, 0x54, 0x94, 0x91, 0x51, 0xD2, 0x12, 0xD8, 0x18, 0x9B, 0x5B, 0x5E, 0x9E, 0x1D, 0xDD,
            0x4A, 0x8A, 0x09, 0xC9, 0xCC, 0x0C, 0x8F, 0x4F, 0x85, 0x45, 0xC6, 0x06, 0x03, 0xC3, 0x40, 0x80,
            0x1A, 0xDA, 0x59, 0x99, 0x9C, 0x5C, 0xDF, 0x1F, 0xD5, 0x15, 0x96, 0x56, 0x53, 0x93, 0x10, 0xD0,
            0x47, 0x87, 0x04, 0xC4, 0xC1, 0x01, 0x82, 0x42, 0x88, 0x48, 0xCB, 0x0B, 0x0E, 0xCE, 0x4D, 0x8D,
            0xA0, 0x60, 0xE3, 0x23, 0x26, 0xE6, 0x65, 0xA5, 0x6F, 0xAF, 0x2C, 0xEC, 0xE9, 0x29, 0xAA, 0x6A,
            0xFD, 0x3D, 0xBE, 0x7E, 0x7B, 0xBB, 0x38, 0xF8, 0x32, 0xF2, 0x71, 0xB1, 0xB4, 0x74, 0xF7, 0x37,
        };

        private static readonly byte[] BP194 = new byte[]
        {
            0x00, 0xC2, 0x47, 0x85, 0x8E, 0x4C, 0xC9, 0x0B, 0xDF, 0x1D, 0x98, 0x5A, 0x51, 0x93, 0x16, 0xD4,
            0x7D, 0xBF, 0x3A, 0xF8, 0xF3, 0x31, 0xB4, 0x76, 0xA2, 0x60, 0xE5, 0x27, 0x2C, 0xEE, 0x6B, 0xA9,
            0xFA, 0x38, 0xBD, 0x7F, 0x74, 0xB6, 0x33, 0xF1, 0x25, 0xE7, 0x62, 0xA0, 0xAB, 0x69, 0xEC, 0x2E,
            0x87, 0x45, 0xC0, 0x02, 0x09, 0xCB, 0x4E, 0x8C, 0x58, 0x9A, 0x1F, 0xDD, 0xD6, 0x14, 0x91, 0x53,
            0x37, 0xF5, 0x70, 0xB2, 0xB9, 0x7B, 0xFE, 0x3C, 0xE8, 0x2A, 0xAF, 0x6D, 0x66, 0xA4, 0x21, 0xE3,
            0x4A, 0x88, 0x0D, 0xCF, 0xC4, 0x06, 0x83, 0x41, 0x95, 0x57, 0xD2, 0x10, 0x1B, 0xD9, 0x5C, 0x9E,
            0xCD, 0x0F, 0x8A, 0x48, 0x43, 0x81, 0x04, 0xC6, 0x12, 0xD0, 0x55, 0x97, 0x9C, 0x5E, 0xDB, 0x19,
            0xB0, 0x72, 0xF7, 0x35, 0x3E, 0xFC, 0x79, 0xBB, 0x6F, 0xAD, 0x28, 0xEA, 0xE1, 0x23, 0xA6, 0x64,
            0x6E, 0xAC, 0x29, 0xEB, 0xE0, 0x22, 0xA7, 0x65, 0xB1, 0x73, 0xF6, 0x34, 0x3F, 0xFD, 0x78, 0xBA,
            0x13, 0xD1, 0x54, 0x96, 0x9D, 0x5F, 0xDA, 0x18, 0xCC, 0x0E, 0x8B, 0x49, 0x42, 0x80, 0x05, 0xC7,
            0x94, 0x56, 0xD3, 0x11, 0x1A, 0xD8, 0x5D, 0x9F, 0x4B, 0x89, 0x0C, 0xCE, 0xC5, 0x07, 0x82, 0x40,
            0xE9, 0x2B, 0xAE, 0x6C, 0x67, 0xA5, 0x20, 0xE2, 0x36, 0xF4, 0x71, 0xB3, 0xB8, 0x7A, 0xFF, 0x3D,
            0x59, 0x9B, 0x1E, 0xDC, 0xD7, 0x15, 0x90, 0x52, 0x86, 0x44, 0xC1, 0x03, 0x08, 0xCA, 0x4F, 0x8D,
            0x24, 0xE6, 0x63, 0xA1, 0xAA, 0x68, 0xED, 0x2F, 0xFB, 0x39, 0xBC, 0x7E, 0x75, 0xB7, 0x32, 0xF0,
            0xA3, 0x61, 0xE4, 0x26, 0x2D, 0xEF, 0x6A, 0xA8, 0x7C, 0xBE, 0x3B, 0xF9, 0xF2, 0x30, 0xB5, 0x77,
            0xDE, 0x1C, 0x99, 0x5B, 0x50, 0x92, 0x17, 0xD5, 0x01, 0xC3, 0x46, 0x84, 0x8F, 0x4D, 0xC8, 0x0A,
        };

        private static readonly byte[] BP251 = new byte[]
        {
            0x00, 0xFB, 0x35, 0xCE, 0x6A, 0x91, 0x5F, 0xA4, 0xD4, 0x2F, 0xE1, 0x1A, 0xBE, 0x45, 0x8B, 0x70,
            0x6B, 0x90, 0x5E, 0xA5, 0x01, 0xFA, 0x34, 0xCF, 0xBF, 0x44, 0x8A, 0x71, 0xD5, 0x2E, 0xE0, 0x1B,
            0xD6, 0x2D, 0xE3, 0x18, 0xBC, 0x47, 0x89, 0x72, 0x02, 0xF9, 0x37, 0xCC, 0x68, 0x93, 0x5D, 0xA6,
            0xBD, 0x46, 0x88, 0x73, 0xD7, 0x2C, 0xE2, 0x19, 0x69, 0x92, 0x5C, 0xA7, 0x03, 0xF8, 0x36, 0xCD,
            0x6F, 0x94, 0x5A, 0xA1, 0x05, 0xFE, 0x30, 0xCB, 0xBB, 0x40, 0x8E, 0x75, 0xD1, 0x2A, 0xE4, 0x1F,
            0x04, 0xFF, 0x31, 0xCA, 0x6E, 0x95, 0x5B, 0xA0, 0xD0, 0x2B, 0xE5, 0x1E, 0xBA, 0x41, 0x8F, 0x74,
            0xB9, 0x42, 0x8C, 0x77, 0xD3, 0x28, 0xE6, 0x1D, 0x6D, 0x96, 0x58, 0xA3, 0x07, 0xFC, 0x32, 0xC9,
            0xD2, 0x29, 0xE7, 0x1C, 0xB8, 0x43, 0x8D, 0x76, 0x06, 0xFD, 0x33, 0xC8, 0x6C, 0x97, 0x59, 0xA2,
            0xDE, 0x25, 0xEB, 0x10, 0xB4, 0x4F, 0x81, 0x7A, 0x0A, 0xF1, 0x3F, 0xC4, 0x60, 0x9B, 0x55, 0xAE,
            0xB5, 0x4E, 0x80, 0x7B, 0xDF, 0x24, 0xEA, 0x11, 0x61, 0x9A, 0x54, 0xAF, 0x0B, 0xF0, 0x3E, 0xC5,
            0x08, 0xF3, 0x3D, 0xC6, 0x62, 0x99, 0x57, 0xAC, 0xDC, 0x27, 0xE9, 0x12, 0xB6, 0x4D, 0x83, 0x78,
            0x63, 0x98, 0x56, 0xAD, 0x09, 0xF2, 0x3C, 0xC7, 0xB7, 0x4C, 0x82, 0x79, 0xDD, 0x26, 0xE8, 0x13,
            0xB1, 0x4A, 0x84, 0x7F, 0xDB, 0x20, 0xEE, 0x15, 0x65, 0x9E, 0x50, 0xAB, 0x0F, 0xF4, 0x3A, 0xC1,
            0xDA, 0x21, 0xEF, 0x14, 0xB0, 0x4B, 0x85, 0x7E, 0x0E, 0xF5, 0x3B, 0xC0, 0x64, 0x9F, 0x51, 0xAA,
            0x67, 0x9C, 0x52, 0xA9, 0x0D, 0xF6, 0x38, 0xC3, 0xB3, 0x48, 0x86, 0x7D, 0xD9, 0x22, 0xEC, 0x17,
            0x0C, 0xF7, 0x39, 0xC2, 0x66, 0x9D, 0x53, 0xA8, 0xD8, 0x23, 0xED, 0x16, 0xB2, 0x49, 0x87, 0x7C,
        };

        #endregion

        private static byte l(byte[] a)
        {
            return (byte)(
                BP148[a[0]] ^ BP032[a[1]] ^ BP133[a[2]] ^ BP016[a[3]] ^
                BP194[a[4]] ^ BP192[a[5]] ^ a[6] ^ BP251[a[7]] ^
                a[8] ^ BP192[a[9]] ^ BP194[a[10]] ^ BP016[a[11]] ^
                BP133[a[12]] ^ BP032[a[13]] ^ BP148[a[14]] ^ a[15]);
        }

        private static void F(byte[] C, byte[] a0, byte[] a1, out byte[] r0, out byte[] r1)
        {
            byte[] buf = new byte[16];

            // X[C](a1)
            for (int i = 0; i < buf.Length; i++)
                buf[i] = (byte)(C[i] ^ a1[i]);

            // SX[C](a1)
            for (int i = 0; i < buf.Length; i++)
                buf[i] = Pi[buf[i]];

            // LSX[C](a1)
            for (int round = 0; round < 16; round++)
            {
                byte lres = l(buf);
                for (int i = 15; i >= 1; i--)
                    buf[i] = buf[i - 1];
                buf[0] = lres;
            }

            // LSX[C](a1) xor a0
            for (int i = 0; i < buf.Length; i++)
                buf[i] ^= a0[i];

            r0 = buf;
            r1 = a1;
        }

        private static void KeyExpansion(byte[] key, out byte[][] K)
        {
            K = new byte[10][];
            K[0] = new byte[16];
            K[1] = new byte[16];

            Array.Copy(key, 0, K[0], 0, 16);
            Array.Copy(key, 16, K[1], 0, 16);

            for (int i = 1; i <= 4; i++)
            {
                byte[] r0, r1;
                byte[] a0 = K[2 * i - 2], a1 = K[2 * i - 1];
                for (int j = 0; j < 8; j++)
                {
                    F(C[8 * (i - 1) + j], a1, a0, out r0, out r1);
                    a0 = r0;
                    a1 = r1;
                }

                K[i * 2] = new byte[16];
                K[i * 2 + 1] = new byte[16];
                Array.Copy(a0, 0, K[i * 2], 0, 16);
                Array.Copy(a1, 0, K[i * 2 + 1], 0, 16);
            }
        }

        public override Func<byte[], byte[]> CreateEncryptor(byte[] key)
        {
            byte[][] K;
            KeyExpansion(key, out K);

            return (block) =>
            {
                byte[] result = new byte[16];
                Array.Copy(block, 0, result, 0, 16);

                for (int round = 0; round < 10; round++)
                {
                    byte[] currentK = K[round];

                    // X[Ki](a)
                    for (int i = 0; i < result.Length; i++)
                        result[i] ^= currentK[i];

                    if (round == 9)
                        break;

                    // SX[Ki](a)
                    for (int i = 0; i < result.Length; i++)
                        result[i] = Pi[result[i]];

                    // LSX[Ki](a)
                    for (int c = 0; c < 16; c++)
                    {
                        byte lres = l(result);
                        for (int i = 15; i >= 1; i--)
                            result[i] = result[i - 1];
                        result[0] = lres;
                    }
                }

                return result;
            };
        }

        public override Func<byte[], byte[]> CreateDecryptor(byte[] key)
        {
            byte[][] K;
            KeyExpansion(key, out K);

            return (block) =>
            {
                byte[] result = new byte[16];
                Array.Copy(block, 0, result, 0, 16);

                for (int round = 9; round >= 0; round--)
                {
                    byte[] currentK = K[round];

                    // X[Ki](a)
                    for (int i = 0; i < result.Length; i++)
                        result[i] ^= currentK[i];

                    if (round == 0)
                        break;

                    // L(-1)SX[Ki](a)
                    for (int c = 0; c < 16; c++)
                    {
                        byte buf = result[0];
                        for (int i = 0; i < 15; i++)
                            result[i] = result[i + 1];
                        result[15] = buf;
                        result[15] = l(result);
                    }

                    // S(-1)X[Ki](a)
                    for (int i = 0; i < result.Length; i++)
                        result[i] = PiInv[result[i]];
                }

                return result;
            };
        }
    }
}