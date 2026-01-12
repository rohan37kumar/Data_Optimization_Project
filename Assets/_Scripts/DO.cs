//Data Optimization - encoding and decoding classes and static methods --> main logic here

using UnityEngine;

//for encoding
public class DOEncoder
{
    private Vector3 lastSentPosition;
    private bool initialized = false;

    public byte[] Encode(Vector3 currentPos)
    {
        if (!initialized)   //for first time, sending full position
        {
            initialized = true;
            lastSentPosition = currentPos;
            return DO.EncodeCoordsToBytes(currentPos, Vector3.positiveInfinity);
        }

        byte[] encodedData = DO.EncodeCoordsToBytes(currentPos, lastSentPosition);

        //here we decode to update lastSentPosition correctly
        Vector3 decoded = DO.DecodeCoordsFromBytes(encodedData);
        byte typeFlag = (byte)((encodedData[0] & 0xC0) >> 6);
        
        if (typeFlag == 0x00 || typeFlag == 0x01)
            lastSentPosition += decoded;  // if delta encoding
        else
            lastSentPosition = decoded;   // for full position encoding

        return encodedData;
    }
}

// for decoding
public class DODecoder
{
    private Vector3 lastReceivedPosition;

    public Vector3 Decode(byte[] data)
    {
        Vector3 decoded = DO.DecodeCoordsFromBytes(data);
        byte typeFlag = (byte)((data[0] & 0xC0) >> 6);

        Vector3 actualPosition;
        //for delta encoding we'll add in last received pos and get actual pos
        if (typeFlag == 0x00 || typeFlag == 0x01)
            actualPosition = lastReceivedPosition + decoded;
        else
            actualPosition = decoded;   // for full position we get actual pos directly

        lastReceivedPosition = actualPosition;
        return actualPosition;
    }
}


//static class for Data Optimization operations
public static class DO
{
    // defining play area bounds [world coords] -> assuming no object goes beyond in local scene setup
    public static Vector3 playAreaMin = new Vector3(-55f, -20f, -30f);
    public static Vector3 playAreaMax = new Vector3(55f, 20f, 30f);

    public static int GetSizeInBits(byte[] data)
    {
        return data.Length * 8;
    }

    public static byte[] EncodeCoordsToBytes(Vector3 currentPos, Vector3 lastSentPosition)
    {
        Vector3 delta = currentPos - lastSentPosition;
        
        // check 1 -> for very small change in movements
        if (Mathf.Abs(delta.x) <= 0.125f && Mathf.Abs(delta.y) <= 0.125f && Mathf.Abs(delta.z) <= 0.125f)
        {
            byte[] data = new byte[1];
            

            int dx = Mathf.RoundToInt(Mathf.Clamp(delta.x / 0.0625f, -2, 1));
            int dy = Mathf.RoundToInt(Mathf.Clamp(delta.y / 0.0625f, -2, 1));
            int dz = Mathf.RoundToInt(Mathf.Clamp(delta.z / 0.0625f, -2, 1));
            
            data[0] = (byte)(0x00 |                 // type flag 00 -> for 1 byte delta
                            ((dx & 0x03) << 4) |
                            ((dy & 0x03) << 2) |
                            (dz & 0x03));
            
            return data; // 8 bits ~ 90% reduction!
        }
        

        else if (Mathf.Abs(delta.x) <= 1.0f && Mathf.Abs(delta.y) <= 1.0f && Mathf.Abs(delta.z) <= 1.0f)
        {
            byte[] data = new byte[2];
            
            int dx = Mathf.RoundToInt(Mathf.Clamp(delta.x / 0.125f, -8, 7));
            int dy = Mathf.RoundToInt(Mathf.Clamp(delta.y / 0.125f, -8, 7));
            int dz = Mathf.RoundToInt(Mathf.Clamp(delta.z / 0.125f, -8, 7));
            
            data[0] = (byte)(0x40 |                // type flag 01 -> for 2 byte delta
                            ((dx & 0x0F) << 2) |
                            ((dy & 0x0C) >> 2));
            data[1] = (byte)(((dy & 0x03) << 6) |
                            ((dz & 0x0F) << 2));
            
            return data; // 16 bits ~ 80% reduction!
        }
        

        else
        {
            byte[] data = new byte[4];

            int x = Mathf.RoundToInt(Mathf.Clamp(
                ((currentPos.x - playAreaMin.x) / (playAreaMax.x - playAreaMin.x)) * 1023f, 0, 1023));
            int y = Mathf.RoundToInt(Mathf.Clamp(
                ((currentPos.y - playAreaMin.y) / (playAreaMax.y - playAreaMin.y)) * 1023f, 0, 1023));
            int z = Mathf.RoundToInt(Mathf.Clamp(
                ((currentPos.z - playAreaMin.z) / (playAreaMax.z - playAreaMin.z)) * 1023f, 0, 1023));
            

            data[0] = (byte)(0x80 | ((x >> 4) & 0x3F)); // Type + X[9:4]
            data[1] = (byte)(((x & 0x0F) << 4) | ((y >> 6) & 0x0F)); // X[3:0] + Y[9:6]
            data[2] = (byte)(((y & 0x3F) << 2) | ((z >> 8) & 0x03)); // Y[5:0] + Z[9:8]
            data[3] = (byte)(z & 0xFF); // Z[7:0]
            
            return data; // 32 bits ~65% reduction atleast, even in the worst case [originally 96 bits]
        }
    }
    
    // Decoding method
    public static Vector3 DecodeCoordsFromBytes(byte[] data)
    {
        // first check encoding type from first 2 bits
        byte typeFlag = (byte)((data[0] & 0xC0) >> 6);
        
        if (typeFlag == 0x00 && data.Length == 1)   //if 1 byte data
        {
            int dx = (int)((data[0] >> 4) & 0x03);
            int dy = (int)((data[0] >> 2) & 0x03);
            int dz = (int)(data[0] & 0x03);
            
            // unsigned to signed (2-bit two's complement)
            if (dx >= 2) dx -= 4;
            if (dy >= 2) dy -= 4;
            if (dz >= 2) dz -= 4;
            
            // return delta
            return new Vector3(dx * 0.0625f, dy * 0.0625f, dz * 0.0625f);
        }
        else if (typeFlag == 0x01 && data.Length == 2)  //if 2 byte data
        {
            int dx = (int)((data[0] >> 2) & 0x0F);
            int dy = (int)(((data[0] & 0x03) << 2) | ((data[1] >> 6) & 0x03));
            int dz = (int)((data[1] >> 2) & 0x0F);
            
            if (dx >= 8) dx -= 16;
            if (dy >= 8) dy -= 16;
            if (dz >= 8) dz -= 16;
            
            // final delta return
            return new Vector3(dx * 0.125f, dy * 0.125f, dz * 0.125f);
        }
        else    // if 4 byte data
        {
            int x = (int)(((data[0] & 0x3F) << 4) | ((data[1] >> 4) & 0x0F));
            int y = (int)(((data[1] & 0x0F) << 6) | ((data[2] >> 2) & 0x3F));
            int z = (int)(((data[2] & 0x03) << 8) | (data[3] & 0xFF));
            
            //here we get full world coords not just delta
            return new Vector3(
                playAreaMin.x + (x / 1023f) * (playAreaMax.x - playAreaMin.x),
                playAreaMin.y + (y / 1023f) * (playAreaMax.y - playAreaMin.y),
                playAreaMin.z + (z / 1023f) * (playAreaMax.z - playAreaMin.z)
            );
        }
    }

}
