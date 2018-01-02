using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Asymmetric;
using Zergatul.Math.EllipticCurves.PrimeField;

namespace Zergatul.Cryptocurrency.Bitcoin.ScriptOpcodes
{
    public class Operator
    {
        public Opcode? Opcode;
        public byte[] Data;

        private static byte[] OP_FALSE = new byte[0];
        private static byte[] OP_TRUE = new byte[] { 1 };

        public bool Run(byte[] verifyBytes, Stack<byte[]> stack)
        {
            if (Opcode == null && Data == null)
                throw new InvalidOperationException();

            if (Opcode == null)
            {
                stack.Push(Data);
                return true;
            }

            if (Data == null)
            {
                switch (Opcode.Value)
                {
                    case ScriptOpcodes.Opcode.OP_1:
                        OP_1(stack);
                        break;

                    case ScriptOpcodes.Opcode.OP_VERIFY:
                        if (!OP_VERIFY(stack))
                            return false;
                        break;

                    case ScriptOpcodes.Opcode.OP_DUP:
                        OP_DUP(stack);
                        break;

                    case ScriptOpcodes.Opcode.OP_EQUAL:
                        OP_EQUAL(stack);
                        break;

                    case ScriptOpcodes.Opcode.OP_EQUALVERIFY:
                        if (!OP_EQUALVERIFY(stack))
                            return false;
                        break;

                    case ScriptOpcodes.Opcode.OP_HASH160:
                        OP_HASH160(stack);
                        break;

                    case ScriptOpcodes.Opcode.OP_CHECKSIG:
                        OP_CHECKSIG(verifyBytes, stack);
                        break;

                    case ScriptOpcodes.Opcode.OP_CHECKMULTISIG:
                        OP_CHECKMULTISIG(verifyBytes, stack);
                        break;

                    default:
                        throw new NotImplementedException();
                }

                return true;
            }

            throw new InvalidOperationException();
        }

        private bool Result(Stack<byte[]> stack)
        {
            if (stack.Count == 0)
                return false;

            byte[] result = stack.Pop();
            for (int i = 0; i < result.Length; i++)
                if (result[i] != 0)
                    return true;

            return false;
        }

        private void OP_1(Stack<byte[]> stack)
        {
            stack.Push(new byte[] { 1 });
        }

        private bool OP_VERIFY(Stack<byte[]> stack)
        {
            return Result(stack);
        }

        private void OP_DUP(Stack<byte[]> stack)
        {
            if (stack.Count == 0)
                throw new InvalidOperationException();

            stack.Push(stack.Peek());
        }

        private void OP_EQUAL(Stack<byte[]> stack)
        {
            if (stack.Count < 2)
                throw new InvalidOperationException();

            byte[] value1 = stack.Pop();
            byte[] value2 = stack.Pop();

            if (ByteArray.Equals(value1, value2))
                stack.Push(OP_TRUE);
            else
                stack.Push(OP_FALSE);
        }

        private bool OP_EQUALVERIFY(Stack<byte[]> stack)
        {
            OP_EQUAL(stack);
            return OP_VERIFY(stack);
        }

        private void OP_HASH160(Stack<byte[]> stack)
        {
            if (stack.Count == 0)
                throw new InvalidOperationException();

            byte[] data = stack.Pop();

            var hash160 = new RIPE160SHA256();
            hash160.Update(data);
            stack.Push(hash160.ComputeHash());
        }

        private void OP_CHECKSIG(byte[] verifyBytes, Stack<byte[]> stack)
        {
            if (stack.Count < 2)
                throw new InvalidOperationException();

            byte[] pubkey = stack.Pop();
            byte[] signature = stack.Pop();

            int hashTypeCode = signature[signature.Length - 1];
            if (hashTypeCode != 1)
                throw new NotImplementedException();
            signature = ByteArray.SubArray(signature, 0, signature.Length - 1);

            byte[] signedData = ByteArray.Concat(verifyBytes, BitHelper.GetBytes(hashTypeCode, ByteOrder.LittleEndian));

            var hash256 = new DoubleSHA256();
            hash256.Update(signedData);
            byte[] hash = hash256.ComputeHash();

            var curve = EllipticCurve.secp256k1;
            var ecdsa = new ECPDSA();
            ecdsa.Parameters = new ECPDSAParameters(curve);
            ecdsa.PublicKey = new ECPPublicKey(ECPoint.FromBytes(pubkey, curve));
            if (ecdsa.VerifyHash(hash, signature))
                stack.Push(OP_TRUE);
            else
                stack.Push(OP_FALSE);
        }

        private void OP_CHECKMULTISIG(byte[] verifyBytes, Stack<byte[]> stack)
        {
            if (stack.Count == 0)
                throw new InvalidOperationException();

            byte[] countBytes = stack.Pop();
            if (countBytes.Length != 1)
                throw new NotImplementedException();

            int count = countBytes[0];
            if (stack.Count < count * 2)
                throw new InvalidOperationException();

            var signatures = new List<byte[]>(count);
            var pubkeys = new List<byte[]>(count);

            for (int i = 0; i < count; i++)
                signatures.Add(stack.Pop());
            for (int i = 0; i < count; i++)
                pubkeys.Add(stack.Pop());

            var curve = EllipticCurve.secp256k1;

            while (signatures.Count > 0)
            {
                byte[] signature = signatures.First();
                signatures.RemoveAt(0);

                int hashTypeCode = signature[signature.Length - 1];
                if (hashTypeCode != 1)
                    throw new NotImplementedException();
                signature = ByteArray.SubArray(signature, 0, signature.Length - 1);

                byte[] signedData = ByteArray.Concat(verifyBytes, BitHelper.GetBytes(hashTypeCode, ByteOrder.LittleEndian));

                var hash256 = new DoubleSHA256();
                hash256.Update(signedData);
                byte[] hash = hash256.ComputeHash();

                bool match = false;
                for (int i = 0; i < pubkeys.Count; i++)
                {
                    var ecdsa = new ECPDSA();
                    ecdsa.Parameters = new ECPDSAParameters(curve);
                    ecdsa.PublicKey = new ECPPublicKey(ECPoint.FromBytes(pubkeys[i], curve));
                    if (ecdsa.VerifyHash(hash, signature))
                    {
                        match = true;
                        pubkeys.RemoveAt(i);
                        break;
                    }
                }

                if (!match)
                {
                    stack.Push(OP_FALSE);
                    return;
                }
            }

            stack.Push(OP_TRUE);
        }
    }
}