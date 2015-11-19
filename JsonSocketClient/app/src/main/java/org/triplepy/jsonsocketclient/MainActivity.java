package org.triplepy.jsonsocketclient;

import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.support.v7.app.AppCompatActivity;
import android.util.Log;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.TextView;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.io.PrintWriter;
import java.net.InetAddress;
import java.net.Socket;

public class MainActivity extends AppCompatActivity implements OnClickListener{

    public static final String windowServerIp = "192.168.1.105";

    public static final int windowServerPort = 1234;

    public static final int REFRESH = 1;

    private Button m_RequestButton;

    private TextView m_TitleTextView;

    private TextView m_ContentTextView;

    private static String title;

    private static String content;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        this.m_RequestButton = (Button)findViewById(R.id.requestButton);

        this.m_TitleTextView = (TextView)findViewById(R.id.titleTextView);
        this.m_ContentTextView = (TextView)findViewById(R.id.contentTextBox);


        m_RequestButton.setOnClickListener(this);
    }


    private void setJsonData(String received){

        try {
            JSONObject jsonObject = new JSONObject(received);
            this.title = jsonObject.getString("title");
            this.content = jsonObject.getString("content");
            Message msg = mainHandler.obtainMessage();
            msg.what = REFRESH;
            mainHandler.sendMessage(msg);
        } catch (JSONException jsone) {
            Log.e("MainActivity", jsone.getMessage());
        }
    }


    private final Handler mainHandler = new Handler() {
        @Override
        public void handleMessage(Message msg) {
            super.handleMessage(msg);

            if(msg.what == REFRESH){
                m_TitleTextView.setText(title);
                m_ContentTextView.setText(content);

            }
        }
    };

    @Override
    public void onClick(View v) {
        if (v.getId() == R.id.requestButton){
            SocketClientThread sct = new SocketClientThread();
            sct.start();
        }
    }


    private class SocketClientThread extends Thread {
        public void run() {
            try {
                InetAddress serverAddr = InetAddress.getByName(windowServerIp);
                Log.d("", "C:Connecting...");

                try {
                    Socket socket = new Socket(serverAddr, windowServerPort);
                    BufferedReader in = new BufferedReader(new InputStreamReader(socket.getInputStream(), "UTF-8"));
                    BufferedWriter out = new BufferedWriter(new OutputStreamWriter(socket.getOutputStream()));
                    PrintWriter pw = new PrintWriter(out, true);

                    pw.println("Request");
                    String json = "";
                    String msg;
                    while (true) {
                        msg = in.readLine();
                        if (!msg.isEmpty() && msg.contains("<End of json>")) break;
                        json += msg;

                    }
                    socket.close();
                    Log.d("ClientActivity", json);
                    Log.d("ClientActivity", "C: Closed.");
                    setJsonData(json);

                } catch (Exception e) {
                    Log.e("ClientActivity", "S: Error", e);
                }


            } catch (Exception e) {
                Log.e("ClientActivity", "S: Error", e);
            }

        }
    }
}
