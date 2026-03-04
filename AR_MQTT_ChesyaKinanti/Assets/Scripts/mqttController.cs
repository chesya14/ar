using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class mqttController : MonoBehaviour
{
	public string nameController = "Controller 1";
	public string tag_mqttManager = ""; // Tag untuk GameObject dengan mqttManager
	public mqttManager _eventSender;

	// Referensi ke TMP_InputField (assign melalui Inspector)
	public TMP_InputField inputField; // Gunakan TMP_InputField untuk input field UI

	// Topik yang ingin ditampilkan (assign melalui Inspector)
	public string selectedTopic = ""; // Ganti dengan topik yang diinginkan

	void Start()
	{
		// Cari GameObject dengan tag yang sesuai
		GameObject[] mqttManagers = GameObject.FindGameObjectsWithTag(tag_mqttManager);
		if (mqttManagers.Length > 0)
		{
			// Ambil komponen mqttManager dari GameObject pertama dengan tag tersebut
			_eventSender = mqttManagers[0].GetComponent<mqttManager>();
			if (_eventSender != null)
			{
				// Tambahkan event listener untuk menerima pesan MQTT
				_eventSender.OnMessageArrived += OnMessageArrivedHandler;
			}
			else
			{
				Debug.LogError($"GameObject with tag '{tag_mqttManager}' does not have an mqttManager component.");
			}
		}
		else
		{
			Debug.LogError($"No GameObject found with tag '{tag_mqttManager}'. Please ensure the tag is assigned correctly.");
		}

		// Validasi apakah TMP_InputField telah di-assign
		if (inputField == null)
		{
			Debug.LogError("TMP_InputField reference is not assigned. Please assign it in the Inspector.");
		}
	}

	private void OnMessageArrivedHandler(mqttObj mqttObject) // mqttObj didefinisikan di mqttManager.cs
	{
		// Pastikan objek mqttObject tidak null
		if (mqttObject == null)
		{
			Debug.LogError("Received null mqttObject. Cannot process message.");
			return;
		}

		// Debug log untuk memastikan pesan diterima
		Debug.Log($"Message from Topic '{mqttObject.topic}' is: {mqttObject.msg}");

		// Periksa apakah topik sesuai dengan yang dipilih
		if (mqttObject.topic == selectedTopic)
		{
			// Perbarui TMP_InputField dengan pesan yang diterima
			if (inputField != null)
			{
				inputField.text = mqttObject.msg; // Masukkan pesan ke input field
			}
			else
			{
				Debug.LogError("TMP_InputField reference is null. Cannot update text.");
			}
		}
		else
		{
			Debug.Log($"Ignoring message from topic '{mqttObject.topic}' as it does not match the selected topic '{selectedTopic}'.");
		}
	}

	private void OnDestroy()
	{
		// Hapus event listener saat objek dihancurkan untuk menghindari memory leak
		if (_eventSender != null)
		{
			_eventSender.OnMessageArrived -= OnMessageArrivedHandler;
		}
	}
}
